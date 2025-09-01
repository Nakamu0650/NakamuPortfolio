using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class G_SpiralGenerator : MonoBehaviour
{
    [SerializeField] float minRadius = 0f,maxRadius=50f,density=1f,heightChange=0f;
    private readonly float tangentValue = 0.5522f;
    private readonly Vector3[] positions = new Vector3[4] { new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f)};
    private void OnValidate()
    {
        SplineContainer spline = GetComponent<SplineContainer>();
        spline.Spline.Clear();
        int max = Mathf.CeilToInt(maxRadius * density * 4f);
        for(int i = 0; i < max; i++)
        {
            float radius = Mathf.Lerp(minRadius, maxRadius, (float)i / max);
            var knot = new BezierKnot();
            knot.Position = (positions[i % 4]* radius)+ (Vector3.up * heightChange*((float)i / max));
            knot.Rotation = Quaternion.Euler(0f, 90*(i%4), 0f);
            knot.TangentIn = new Unity.Mathematics.float3(0f,0f, -radius * tangentValue);
            knot.TangentOut = new Unity.Mathematics.float3(0f,0f, radius * tangentValue);

            spline.Spline.Add(knot);
        }
    }
}
