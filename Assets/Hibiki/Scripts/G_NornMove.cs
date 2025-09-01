using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_NornMove : MonoBehaviour
{
    public static G_NornMove instance;
    public G_NornAnimator animator;

    public Vector3 body { get { return bodyOffset; } set { bodyOffset = value; } }
    public Vector3 aim { get { return aimOffset; } set { aimOffset = value; } }

    [SerializeField] Transform followTarget, lookAtTarget;

    [SerializeField] Vector3 bodyOffset, aimOffset;
    [SerializeField] float approachRadis = 1f;
    [SerializeField] float approachSpeed = 1f;
    [SerializeField] float approachMaxSpeed = 1f;

    float sqrRadius;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sqrRadius = Mathf.Pow(approachRadis, 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 agentPosition = followTarget.position + bodyOffset;
        Vector3 vector = (transform.position - agentPosition);
        Vector3 goalPosision = agentPosition + (vector.normalized * approachRadis);
        transform.position += Vector3.ClampMagnitude(Vector3.Lerp(Vector3.zero, goalPosision - transform.position, approachSpeed * Time.fixedDeltaTime), approachMaxSpeed * Time.fixedDeltaTime);

        transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation((lookAtTarget.position + aimOffset) - transform.position).eulerAngles.y, 0f);
    }

    public void SetFollow(Transform follow)
    {
        followTarget = follow;
    }

    public void SetLookAt(Transform lookAt)
    {
        lookAtTarget = lookAt;
    }

    public Transform GetFollow()
    {
        return followTarget;
    }

    public Transform GetLookAt()
    {
        return lookAtTarget;
    }
}
