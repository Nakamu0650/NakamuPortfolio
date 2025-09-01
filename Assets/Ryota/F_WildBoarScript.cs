using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class F_WildBoarScript : MonoBehaviour
{
    [SerializeField] F_Param parameter;
    [SerializeField] F_AttackValue rushAttack;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackTransform;
    [SerializeField] string eunSpeeedAnimation, isRunAnimation, onAttackAnimation, onDamageAnimation;
    [SerializeField] float accelerateTime,runTime;
    [SerializeField] float rotateSpeed,rushSpeed;
    [SerializeField] Searcher searcher;
    private HanadayoRigidobody rb;
    private F_HP playerHP;
    private Vector3 moveVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<HanadayoRigidobody>();
        moveVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        OnMove();
    }

    public void OnPunish()
    {
        StopAllCoroutines();
        P_CameraMove_Alpha.instance.AddAngryEnemy(transform);
        StartCoroutine(Attack());
        animator.SetBool(isRunAnimation, true);
    }
    public void OnKilled()
    {
        StopAllCoroutines();
        Destroy(searcher);
        S_SEManager.PlayEnemyDeatheSE(transform);
        animator.SetTrigger(onDamageAnimation);
    }

    private IEnumerator Attack()
    {
        Transform player = searcher.player;
        bool attacked = false;
        while(searcher.isPunish)
        {
            attacked = false;
            float waitTime = accelerateTime - 1f;
            animator.SetFloat(eunSpeeedAnimation, 1f);
            for (float f = 0; f < waitTime; f += Time.fixedDeltaTime)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(player.position - transform.position, transform.up).eulerAngles.y, 0f), Time.fixedDeltaTime * rotateSpeed);
                yield return new WaitForFixedUpdate();
            }
            animator.SetFloat(eunSpeeedAnimation, 2f);
            yield return new WaitForSeconds(1f);

            for (float f = 0; f < runTime; f += Time.fixedDeltaTime)
            {
                moveVelocity = transform.forward * rushSpeed;

                if (!attacked)
                {
                    Collider[] col = Physics.OverlapBox(attackTransform.position, attackTransform.localScale, attackTransform.rotation).Where(value => value.gameObject.CompareTag("Player")).ToArray();
                    if (col.Length != 0)
                    {
                        if (!playerHP) { playerHP = col[0].gameObject.GetComponent<F_HP>(); }
                        playerHP.OnDamage(parameter.GetATK(), rushAttack.attackValues[0], HanadayoRigidobody.GetPushAxis(transform.position, player.transform.position));
                        attacked = true;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
        P_CameraMove_Alpha.instance.RemoveAngryEnemy(transform);
        animator.SetBool(isRunAnimation, false);
    }

    private void OnMove()
    {
        if (rb.isKnockBack || !rb.onGround) return;
        Vector3 axis = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (axis != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(axis, transform.up).eulerAngles.y, 0f), Time.fixedDeltaTime * rotateSpeed);
        }
        rb.GetRigidbody().velocity = new Vector3(moveVelocity.x, rb.GetRigidbody().velocity.y, moveVelocity.z);
        moveVelocity = Vector3.zero;
    }

}
