using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PVCrowStart : MonoBehaviour
{
    [SerializeField] float startTime;
    private SplineAnimate splineAnimate;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();

        yield return new WaitForSeconds(startTime);

        splineAnimate.Play();
    }
}
