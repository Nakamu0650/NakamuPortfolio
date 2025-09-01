using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class G_ScrewFlower : MonoBehaviour
{
    private Vector2[] circlePosBase = new Vector2[4] { -Vector2.right, Vector2.up, Vector2.right, -Vector2.up };
    private float circleTangent = 0.2761424f;

    [SerializeField]
    Transform stemTransform;
    [Header("調整値")]
    [SerializeField] SplineContainer spline;
    [SerializeField] float flowerSlant = 25f;
    [SerializeField] float flowerHeight = 25f;
    [SerializeField] float flowerRadius = 5f;
    [SerializeField] float colliderBorderRange = 1f;
    [SerializeField] float stemRadius = 2f;

    private void OnValidate()
    {
        if (flowerSlant == 0 || flowerRadius == 0) return;
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        col.radius = flowerRadius + 2 * colliderBorderRange;
        col.height = flowerHeight + col.radius;
        col.center = new Vector3(0f, col.height / 2f - col.radius, 0f);
        stemTransform.localScale = new Vector3(stemRadius, flowerHeight - 2 * flowerRadius, stemRadius);

        float pointHeight = 0.5f * flowerRadius * Mathf.PI * Mathf.Tan(flowerSlant * Mathf.Deg2Rad);
        float slantX = 360f - flowerSlant;

        spline.Spline.Clear();
        for (int i = 0; i < (col.height - 3 * flowerRadius) / pointHeight; i++)
        {
            Vector3 pos = new Vector3(flowerRadius * circlePosBase[i % 4].x, i * pointHeight, flowerRadius * circlePosBase[i % 4].y);
            BezierKnot knot = new BezierKnot();
            knot.Position = pos;
            Quaternion rot = Quaternion.Euler(slantX, 90f * (i % 4), 0f);
            knot.Rotation = rot;
            knot.TangentIn = -2 * circleTangent * flowerRadius * Vector3.forward;
            knot.TangentOut = 2 * circleTangent * flowerRadius * Vector3.forward;
            spline.Spline.Add(knot);
        }

    }
    private Transform playerTransform;
    private P_PlayerMove playerMove;
    private bool isEnter;
    [Header("プレイヤー")]
    [SerializeField] float maxNearestPointDistance = 1f;
    [SerializeField] float minMatchValue = 0.7f;
    [SerializeField] float speedBuffPower = 1.25f;

    // Start is called before the first frame update
    void Start()
    {
        isEnter = false;
    }

    void FixedUpdate()
    {
        if (!isEnter) return;

        Vector3 playerPos = playerTransform.position - Vector3.up*playerTransform.localScale.y;

        //Get "Distance" and "NearestPoint for spline" and "NearestPoint / SplineLength"
        float distance = SplineUtility.GetNearestPoint(spline.Spline, playerPos-transform.position, out var nearestPoint, out float t);

        if (distance > maxNearestPointDistance) return;

        //Get NearestPoint's tangent(Update Y to 0)
        spline.Spline.Evaluate(t, out var position, out var tangent, out var upVector);
        Vector3 tangentVec = new Vector3(tangent.x, 0f, tangent.z).normalized;

        float matchValue = Vector3.Dot(playerTransform.forward, tangentVec);
        if (Mathf.Abs(matchValue) < minMatchValue) return;

        playerMove.OverWritePlayerPosition(tangentVec*(matchValue>0?1f:-1f));
        //playerMove.speedBuffs.Add(new G_PlayerMoveBaff.Speed(speedBuffPower, Time.fixedDeltaTime));


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!playerMove || !playerTransform)
            {
                playerTransform = other.transform;
                playerMove = other.GetComponent<P_PlayerMove>();
            }
            isEnter = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isEnter = false;
        }
    }
}
