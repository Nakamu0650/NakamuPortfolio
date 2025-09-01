using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;


public class G_NornPoint : MonoBehaviour
{
    [SerializeField] Transform follow, lookAt;
    [SerializeField] Vector3 bodyOffset, aimOffset;
    [SerializeField] Events events;

    private G_NornMove norn;
    private Transform beforeFollow, beforeLookAt;
    private Vector3 beforeBodyOfset, beforeAimOfset;
    // Start is called before the first frame update
    void Start()
    {
        norn = G_NornMove.instance;
    }

    public void SetFollowTarget()
    {
        SetBefores();
        norn.SetFollow(follow);
        norn.body = bodyOffset;
        events.onSet.Invoke();
    }

    public void SetLookAtTarget()
    {
        SetBefores();
        norn.SetLookAt(lookAt);
        norn.aim = aimOffset;
        events.onSet.Invoke();
    }

    public void SetBothTarget()
    {
        SetBefores();
        norn.SetFollow(follow);
        norn.SetLookAt(lookAt);
        norn.body = bodyOffset;
        norn.aim = aimOffset;
        events.onSet.Invoke();
    }

    public void ReSetFollowTarget()
    {
        if (beforeFollow)
        {
            norn.SetFollow(beforeFollow);
            norn.body = beforeBodyOfset;
            events.onReset.Invoke();
        }
    }

    public void ReSetLookAtTarget()
    {
        if (beforeLookAt)
        {
            norn.SetLookAt(beforeLookAt);
            norn.aim = beforeAimOfset;
            events.onReset.Invoke();
        }
    }

    public void ResetBothTarget()
    {
        if (beforeFollow)
        {
            norn.SetFollow(beforeFollow);
            norn.body = beforeBodyOfset;
        }
        if (beforeLookAt)
        {
            norn.SetLookAt(beforeLookAt);
            norn.aim = beforeAimOfset;
        }
        events.onReset.Invoke();
    }

    private void SetBefores()
    {
        beforeFollow = norn.GetFollow();
        beforeLookAt = norn.GetLookAt();
        beforeBodyOfset = norn.body;
        beforeAimOfset = norn.aim;
    }

    [Serializable]
    public class Events
    {
        public UnityEvent onSet, onReset;
    }
}
