using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PV_CalenduraDollyAnimation : MonoBehaviour
{
    [SerializeField] AnimationCurve dollyNormalizedPosition;
    [SerializeField] float time;
    [SerializeField] float max;

    private CinemachineDollyCart cart;

    private void Start()
    {
        cart = GetComponent<CinemachineDollyCart>();
    }

    public void OnAnimate()
    {
        StartCoroutine(animate());
        IEnumerator animate()
        {
            for(float f = 0f;f<1f;f+=Time.deltaTime / time)
            {
                cart.m_Position = dollyNormalizedPosition.Evaluate(f) * max;
                yield return null;
            }
        }
    }
}
