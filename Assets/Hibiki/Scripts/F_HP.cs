using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(P_DamageManager))]
public class F_HP : MonoBehaviour
{
    [HideInInspector] public HanadayoRigidobody rb { get { return m_rb; } }
    [SerializeField] public F_Param enemyPram;
    public bool useKnockBack = true;
    [Range(0f, 1f)] public float knockBackThreshould = 0f;
    [SerializeField] public UnityEventWithDamage onDamage;
    [SerializeField] public UnityEvent killedEvents;
    [SerializeField] public UnityEvent damagedEvents;
    [SerializeField] public UnityEvent healedEvents;
    [SerializeField] public UnityEvent halfHpEvents;
    [HideInInspector] public bool isInvincible=false;
    [HideInInspector] public bool isKilled;
    [HideInInspector] public int HP, maxHP;
    private int DEF, maxDEF;

    private P_DamageManager damageManager;
    private G_HitStopGenerator hitStop;
    private G_ShakeCamera shakeCamera;
    private HanadayoRigidobody m_rb;

    bool isHalf;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
        isHalf = false;
        damageManager = GetComponent<P_DamageManager>();
        hitStop = G_HitStopGenerator.instance;
        shakeCamera = G_ShakeCamera.instance;

        if (useKnockBack)
        {
            m_rb = GetComponent<HanadayoRigidobody>();
            if(m_rb == null)
            {
                Debug.LogError($"{gameObject.name}にはHanadayoRigidbodyが付与されていないためノックバックを実行できません", this);
                useKnockBack = false;
            }
        }
    }

    public bool OnDamage(int ATK, F_AttackValue.Attack attackValue, Vector3 axis)
    {
        return OnDamage(ATK, attackValue.strength, attackValue.pushThreshould, attackValue.pushPower * axis);
    }

    public bool OnDamage(int ATK, int STR, float threshould, Vector3 pushVelocity)
    {
        if (isKilled) return false;
        var _damage = damageManager.DamageCalculation(ATK, STR, DEF);
        onDamage.Invoke(_damage);

        if (isInvincible) return false;

        damageManager.CurrentDamage = _damage;
        HP -= damageManager.CurrentDamage;
        if(HP <= maxHP/2&&isHalf== false)
        {
            halfHpEvents.Invoke();
            isHalf = true;
        }
        //S_SEManager.PlayEnemyDamageSE(transform);
        hitStop.Stop(damageManager.CurrentDamage);
        if (gameObject.CompareTag("Enemy"))
        {
            shakeCamera.DamageShake(damageManager.CurrentDamage);
        }
        else
        {
            S_SEManager.PlayPlayerDamageSE(transform);
        }

        Debug.Log($"ノックバックが{((threshould >= knockBackThreshould) ? "呼ばれる" : "呼ばれない")}", this);
        if (useKnockBack && (threshould >= knockBackThreshould))
        {
            m_rb.KnockBack(pushVelocity);
        }

        damagedEvents.Invoke();
        if (HP <= 0)
        {
            Kill();
            return true;
        }
        return false;
    }

    public void OnHeal(int amount)
    {
        HP = Mathf.Min(HP + amount, maxHP);
        healedEvents.Invoke();
    }

    public void Reset()
    {
        maxHP = enemyPram.GetHP();
        HP = maxHP;
        isKilled = false;
        maxDEF = enemyPram.GetDEF();
        DEF = maxDEF;
    }

    public void Kill()
    {
        if (isKilled) return;
        HP = 0;
        isKilled = true;
        killedEvents.Invoke();
    }

    public float GetHPPercent()
    {
        return (float)HP / (float)maxHP;
    }

    [Serializable]
    public class UnityEventWithDamage : UnityEvent<int>
    {

    }
}
