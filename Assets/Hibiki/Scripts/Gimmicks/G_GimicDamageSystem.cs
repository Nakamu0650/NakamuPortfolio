using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class G_GimicDamageSystem : MonoBehaviour
{
    [SerializeField] G_GimicDamageEvent onDamageEvents;
    public void OnDamage(int _damage, P_UmbrellaAttackScript.AttackType _attackType)
    {
        onDamageEvents.Invoke(_damage,_attackType);
    }
}
[Serializable]
public class G_GimicDamageEvent : UnityEvent<int, P_UmbrellaAttackScript.AttackType> { }
