using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class G_GentianHoldAttack : G_UmbrellaHoldAttack
{
    [SerializeField] AttackValues values;
    [SerializeField] F_HP playerHP;
    [SerializeField] Vector3 effectPivot;
    [SerializeField] GameObject shildEffect, destroyEffect, breakEffect;
    [SerializeField] string isOnAnimation, speedAnimation;

    private int endurationValue;
    private bool isHolding;
    private bool isActive;
    private bool isParryale = true;
    private GameObject instanceShield;
    private Animator animator;

    // Set is called before the first frame update
    public override void Set()
    {
        isHolding = false;
        isParryale = false;
        isActive = false;
        animator = P_PlayerMove.instance.modelAnimator;
        playerHP.onDamage.AddListener(OnDamage);
    }

    /// <summary>
    /// 重撃開始
    /// </summary>
    public override void OnAttack()
    {
        if (isActive)
        {
            return;
        }
        base.OnAttack();
        isHolding = true;
        isParryale = false;
        StartCoroutine(HoldingShield());

        G_BloomProvidenceAnalysisSystem.instance.data.gentian++;
    }

    /// <summary>
    /// 銃撃終わり
    /// </summary>
    public override void OnAttackEnd()
    {
        base.OnAttackEnd();
        isHolding = false;
        isParryale = false;
    }

    /// <summary>
    /// シールド展開中の処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator HoldingShield()
    {
        animator.SetFloat(speedAnimation, 1f / values.beforeGapDuration);
        SetNumber(1);
        instanceShield = Instantiate(shildEffect, transform.position + transform.TransformDirection(effectPivot), transform.rotation);
        var size = instanceShield.transform.localScale;
        instanceShield.transform.localScale = Vector3.zero;
        instanceShield.transform.DOScale(size, values.beforeGapDuration).SetEase(Ease.InOutSine);
        yield return StartCoroutine(ShieldOpen());


        float time = 0f;
        isParryale = true;
        while (isHolding)
        {
            if (isParryale)
            {
                time += Time.deltaTime;
                if(time > values.parryableDuration)
                {
                    isParryale = false;
                }
            }
            if(endurationValue <= 0)
            {
                animator.SetFloat(speedAnimation, 1f / values.afterGapDuration);
                SetNumber(3);
                Destroy(instanceShield);
                S_SEManager.PlayShieldBreakeSE(transform);
                Instantiate(breakEffect, transform.position + transform.TransformDirection(effectPivot), transform.rotation);
                yield return StartCoroutine(ShieldClose());
                yield break;
            }
            yield return null;
        }

        animator.SetFloat(speedAnimation, 1f / values.afterGapDuration);
        SetNumber(2);
        Destroy(instanceShield);
        Instantiate(destroyEffect, transform.position + transform.TransformDirection(effectPivot), transform.rotation);
        yield return StartCoroutine(ShieldClose());
    }

    /// <summary>
    /// シールド展開
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShieldOpen()
    {
        endurationValue = values.shieldMaxEndurationValue;
        playerMove.canMove = false;
        playerMove.isKinematic = true;
        isActive = true;
        S_SEManager.PlayShieldSE(transform);
        yield return new WaitForSeconds(values.beforeGapDuration);

        playerHP.isInvincible = true;
    }

    /// <summary>
    /// シールド格納
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShieldClose()
    {
        playerHP.isInvincible = false;
        yield return new WaitForSeconds(values.afterGapDuration);

        playerMove.canMove = true;
        playerMove.isKinematic = false;
        isActive = false;
    }

    /// <summary>
    /// HPScriptでダメージを受けた時に呼ばれる
    /// </summary>
    /// <param name="damage"></param>
    public void OnDamage(int damage)
    {
        if (isParryale)
        {
            OnParry();
        }
        endurationValue -= damage;
    }

    /// <summary>
    /// パリィ成功時に呼ばれる
    /// </summary>
    private void OnParry()
    {
        OnAttackEnd();

        animator.SetFloat(speedAnimation, 10f);
        SetNumber(2);
        Destroy(instanceShield);
        Instantiate(destroyEffect, transform.position + transform.TransformDirection(effectPivot), transform.rotation);
        StopAllCoroutines();
        Debug.Log("パリィ成功");
        S_SEManager.PlayShieldParySE(transform);
        playerHP.isInvincible = false;
        isActive = false;
        playerMove.isKinematic = false;
        playerMove.canMove = true;
    }

    private void SetNumber(int num)
    {
        animator.SetInteger(isOnAnimation, num);
    }

    [Serializable]
    private class AttackValues
    {
        //シールドの耐久値
        public int shieldMaxEndurationValue;

        //パリィ判定期間
        public float parryableDuration;

        //前隙、後隙
        public float beforeGapDuration, afterGapDuration;
    }
}
