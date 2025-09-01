using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class G_ExaminableGimmick : MonoBehaviour
{
    [SerializeField] ExaminableGimmickEvent examineEvent;
    public void OnExamined(Transform player)
    {
        examineEvent.Invoke(player);
    }

}
[Serializable]
public class ExaminableGimmickEvent : UnityEvent<Transform> { }
