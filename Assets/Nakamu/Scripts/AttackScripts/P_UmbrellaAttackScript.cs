using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class P_UmbrellaAttackScript : MonoBehaviour
{
    /// <summary>
    /// AttackTypes
    /// </summary>
    public enum AttackType
    {
        Physics, Sunflower, Rose, Calendula
    }

    //AttackType can only choice physics
    AttackType _physics = AttackType.Physics;

    //PlayerParameter
    [SerializeField] P_PlayerParam playerParam;
    private int ATK, maxATK;

    //UmbrellaStrength
    private int STR;

    //About Animation
    [SerializeField] Animator anim;
    [SerializeField] string _attack;

    //Attack overlap
    [SerializeField] Transform attackTrans;
    [SerializeField] float radius = 0.04f;


    void Start()
    {
        maxATK = playerParam.ATK;
        ATK = maxATK;
        STR = 10;
    }

    // This is ParentClass about AtackType.
    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            anim.SetBool(_attack, true);
            Debug.Log("UmbrellaAtack");
            OnAttack();
        }
        else anim.ResetTrigger(_attack);
    }

    /*
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy" )
        {
            Debug.Log("?U??????????");
            col.gameObject.GetComponent<F_HP>().OnDamage(ATK, STR);
        }
        else if (col.gameObject.CompareTag("VincibleGimic"))
        {
            //col.gameObject.GetComponent<G_GimicDamageSystem>().OnDamage(_damage, _physics);
        }
    }
    */
    
    private void OnAttack()
    {
        var sphereVec = new Vector3(attackTrans.position.x, attackTrans.position.y, attackTrans.position.z);
        Collider[] cols = Physics.OverlapSphere(sphereVec, radius).Where(value => value.gameObject.CompareTag("Enemy")).ToArray();
        foreach(var col in cols)
        {
            Debug.Log("?U??????????");
            if (col.gameObject.CompareTag("Enemy"))
            {
                //col.GetComponent<F_HP>().OnDamage(ATK, STR);
            }
        }
    }
}
