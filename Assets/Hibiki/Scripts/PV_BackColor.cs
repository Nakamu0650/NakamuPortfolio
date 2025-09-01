using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PV_BackColor : MonoBehaviour
{
    [SerializeField] List<Renderer> renderers;
    [SerializeField] Color startColor, endColor;
    [SerializeField] float time;
    [SerializeField] AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        renderers.ForEach(r => r.material.color = startColor);
    }

    public void ValueChange()
    {
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            for(float f = 0;f<1f;f += Time.deltaTime / time)
            {
                renderers.ForEach(r => r.material.color = Color.Lerp(startColor,endColor,curve.Evaluate(f)));
                yield return null;
            }


        }
    }
}
