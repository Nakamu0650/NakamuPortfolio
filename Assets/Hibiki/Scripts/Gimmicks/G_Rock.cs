using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class G_Rock : MonoBehaviour
{
    public enum RockType
    {
        Anything,OnlyCalendula,CantBreak
    }
    [SerializeField] RockType rockType;
    [SerializeField] int maxHP;
    [SerializeField] CinemachineImpulseSource shaker;
    private int HP;
    // Start is called before the first frame update
    void Start()
    {
        if (rockType == RockType.CantBreak) Destroy(this);
        HP = maxHP;
    }

    public void OnDamage(int damage, P_UmbrellaAttackScript.AttackType _attackType)
    {
        switch (rockType)
        {
            case RockType.Anything:
                HP -= ((_attackType == P_UmbrellaAttackScript.AttackType.Calendula ? 2 : 1) * damage);
                break;
            case RockType.OnlyCalendula:
                HP -= (_attackType == P_UmbrellaAttackScript.AttackType.Calendula ? damage : 0) ;
                break;
            default:
                break;
        }
        if (HP <= 0)
        {
            shaker.GenerateImpulse(1f);
            Destroy(gameObject);
        }
    }
}
