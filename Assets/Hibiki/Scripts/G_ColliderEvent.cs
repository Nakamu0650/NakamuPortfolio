using UnityEngine;
using UnityEngine.Events;
using System;

public class G_ColliderEvent : MonoBehaviour
{
    public enum Area
    {
        Area0,
        Area1,
        Area2,
        Area3,
        Area4,
        Area5,
        Area6
         
    }
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }
    public string targetTag = "Untagged";
    public Area area;
    [Header("Trigger Event")]
    public ColliderEvent onTriggerEnter = new ColliderEvent();
    public ColliderEvent onTriggerExit = new ColliderEvent();

    

    void InvokeIfValid(ColliderEvent _handler, Collider _)
    {
        if (_.CompareTag(targetTag))
        {
            _handler.Invoke(_);
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        InvokeIfValid(onTriggerEnter, _other);
    }

    void OnTriggerExit(Collider _other)
    {
        InvokeIfValid(onTriggerExit, _other);
    }
}

