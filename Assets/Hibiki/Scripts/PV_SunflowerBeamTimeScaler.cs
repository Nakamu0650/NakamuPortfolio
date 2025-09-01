using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PV_SunflowerBeamTimeScaler : MonoBehaviour
{
    [SerializeField] AnimationCurve timeScale;
    [SerializeField] float time;

    public void OnTimeScale()
    {
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            for(float f = 0f;f < 1f;f += Time.deltaTime / time)
            {
                Time.timeScale = timeScale.Evaluate(f);
                yield return null;
            }
        }
    }
    
}
