using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class G_Ivy_Pro : G_Ivy
{
#if UNITY_EDITOR
    [SerializeField] SplineContainer spline;
    [SerializeField] float swellValue = 1;
    [SerializeField] float height = 2f;
    [SerializeField] int jointAmount = 4;
    [SerializeField] bool updateIvy = false;
    private void OnValidate()
    {
        if (!updateIvy) return;
        updateIvy = false;
        SplineInstantiate instantiate = GetComponent<SplineInstantiate>();
        spline.Spline.Clear();
        spline.Spline.Add(new BezierKnot(new float3(0, 0, 0)));
        for (int i = 0; i < jointAmount; i++)
        {
            Vector2 pos = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * swellValue;
            spline.Spline.Add(new BezierKnot(new float3(pos.x, (i+1) * height / (float)jointAmount, pos.y)));
        }
        instantiate.Randomize();
    }
#endif
}
