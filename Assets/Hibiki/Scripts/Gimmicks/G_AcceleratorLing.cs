using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class G_AcceleratorLing : MonoBehaviour
{
    [SerializeField] Color passColor, missColor, successColor;
    [SerializeField] float waitTime = 3f;
    private float elapseTime;
    private List<G_Accelerator> accelerators = new List<G_Accelerator>();
    private List<List<Renderer>> renderers = new List<List<Renderer>>();
    private bool checking;
    // Start is called before the first frame update
    void Start()
    {
        checking = false;
        elapseTime = 0f;
        accelerators = new List<G_Accelerator>();
        renderers = new List<List<Renderer>>();
        foreach (Transform child in transform)
        {
            accelerators.Add(child.GetComponent<G_Accelerator>());
            renderers.Add(accelerators.Last().modelRenderer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!checking)
        {
            bool[] passes = accelerators.Select(value => value.isPassed).ToArray();
            if (passes.Contains(true))
            {
                StartCoroutine(CheckLing(Array.IndexOf(passes, true)));
                checking = true;
            }
        }
        else
        {
            elapseTime += Time.deltaTime;
        }

    }
    private IEnumerator CheckLing(int startNumber)
    {
        renderers[startNumber].ForEach(value=>value.material.DOColor(passColor, 0.25f));
        for(int i = 0; i < accelerators.Count-1; i++)
        {
            accelerators[(startNumber + i) % accelerators.Count].isPassed = false;
            yield return new WaitUntil(() => (accelerators.Select(value => value.isPassed).Contains(true))||(elapseTime>=waitTime));
            elapseTime = 0;
            int num = (startNumber + i + 1) % accelerators.Count;
            if (accelerators[num].isPassed)
            {
                //Case Pass
                if(i!= accelerators.Count - 2) renderers[num].ForEach(value => value.material.DOColor(passColor, 0.25f));
            }
            else
            {
                for(int u = 0; u < accelerators.Count; u++)
                {
                    accelerators[u].isPassed = false;
                    renderers[u].ForEach(value=> value.material.color = missColor);
                }
                //Case miss
                checking = false;
                yield return new WaitForSeconds(2f);
                renderers.ForEach(value=>value.ForEach(v=>v.material.DOColor(Color.white, 1f)));
                yield break;
            }
        }
        for (int u = 0; u < accelerators.Count; u++)
        {
            accelerators[u].isPassed = false;
            renderers[u].ForEach(value=>value.material.color = successColor);
        }
        //Case Success
        checking = false;
        G_TrackRecordManager.instance.AchiveTrackRecord("AcceleratorAroundLake");
        yield return new WaitForSeconds(2f);
        renderers.ForEach(value => value.ForEach(v=>v.material.DOColor(Color.white, 1f)));
    }
}
