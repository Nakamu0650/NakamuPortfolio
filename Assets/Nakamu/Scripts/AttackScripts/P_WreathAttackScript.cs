using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_WreathAttackScript : P_UmbrellaAttackScript
{
    //There is enum'script about AttackTypes in "P_UmbrellaAttackScript"
    [Header("AttackTypes")]
    [SerializeField] P_UmbrellaAttackScript.AttackType _type;

    //PlayerParameter
    [SerializeField] P_PlayerParam playerParam;
    private int ATK, maxATK;
    private int STR;

    private void Start()
    {
        maxATK = playerParam.ATK;
        ATK = maxATK;
    }

    // This is ChildClass about WreathAttack.
    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("WreathAtack");
        }
    }

    public new void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Debug.Log("?U??????????");
            //col.gameObject.GetComponent<F_HP>().OnDamage(ATK, STR);
        }
        else if (col.gameObject.CompareTag("VincibleGimic"))
        {
            //col.gameObject.GetComponent<G_GimicDamageSystem>().OnDamage(_damage, _type);
        }
    }
}
