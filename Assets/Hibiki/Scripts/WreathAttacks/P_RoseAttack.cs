using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class P_RoseAttack : P_WreathAttack_Base
{
    [SerializeField] GameObject swordObject;
    [SerializeField] string normalAttackAnimation,strageAnimation, rotationAttackAnimation ;
    [SerializeField] float minStrageTime = 0.5f, maxStrageTime = 3f;
    [SerializeField] float minAttackDuration = 0.5f, maxAttackDuration = 2f;
    [SerializeField] float chainResetTime = 1f;
    [SerializeField] float rotateEndTime = 1f;
    [SerializeField] int useEnergyAmount = 1;
    [SerializeField] float holdMaxCoolTime = 3f;
    [SerializeField] float holdEnergyUseSpan = 0.5f;
    [SerializeField] float moveDuration = 0.25f;

    private Dictionary<GameObject, F_HP> enemyHps;
    private Collider swordCollider;

    private bool activate;

    private int chain;
    private float chainTime;
    private float holdCoolTime;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        activate = false;
        chain = 0;
        holdCoolTime = 0f;
        enemyHps = new Dictionary<GameObject, F_HP>();
        swordCollider = swordObject.GetComponent<Collider>();
    }

    new void Update()
    {
        base.Update();

        ChainTime();
        HoldCoolTime();
    }

    private void ChainTime()
    {
        if (chain == 0)
        {
            return;
        }
        chainTime += Time.deltaTime;
        if (chainTime >= chainResetTime)
        {
            chain = 0;
        }
    }

    private void HoldCoolTime()
    {
        if (holdCoolTime == 0f) return;
        holdCoolTime -= Time.deltaTime;
        if(holdCoolTime <= 0f)
        {
            holdCoolTime = 0f;
        }
    }

    public override void OnPressButton()
    {
        if (playerMove.modelAnimator.GetBool(normalAttackAnimation)) return;

        if (!UseEnergy(useEnergyAmount)) return;

        base.OnPressButton();


        G_BloomProvidenceAnalysisSystem.instance.data.rose++;

        MoveToNearestEnemy(moveDuration);

        playerMove.modelAnimator.SetBool(normalAttackAnimation,true);
    }

    public override void OnHoldButton()
    {
        if(holdCoolTime != 0f)
        {
            CancelHold();
            return;
        }
        if (!UseEnergy(useEnergyAmount))
        {
            CancelHold();
            return;
        }

        base.OnHoldButton();
        S_SEManager.PlayRoseAttackChargeSE(transform);
        StartCoroutine(Attack());

        G_BloomProvidenceAnalysisSystem.instance.data.rose++;

        IEnumerator Attack()
        {
            activate = false;
            playerMove.modelAnimator.SetBool(strageAnimation,true);
            playerMove.isKinematic = true;

            for(float f = 0f;f < minStrageTime; f += Time.deltaTime)
            {
                if (activate)
                {
                    playerMove.modelAnimator.SetBool(strageAnimation, false);
                    playerMove.isKinematic = false;
                    
                    yield break;
                }

                yield return null;
            }
            swordObject.SetActive(true);

            float strageTime = minStrageTime;
            int useEnergy = 0;
            
            while (!activate)
            {
                strageTime += Time.deltaTime;
                if(strageTime >= maxStrageTime)
                {
                    strageTime = maxStrageTime;
                    break;
                }
                int i = Mathf.FloorToInt(strageTime / holdEnergyUseSpan);
                if (i > useEnergy)
                {
                    useEnergy = i;
                    if (!UseEnergy(useEnergyAmount))
                    {
                        break;
                    }
                }

                yield return null;
            }
            holdCoolTime = holdMaxCoolTime;

            float clamp = Mathf.InverseLerp(minStrageTime, maxStrageTime, strageTime);
            float attackDuration = Mathf.Lerp(minAttackDuration, maxAttackDuration, clamp);
            swordCollider.enabled = true;
            S_SEManager.SEStop("roseattackCharge");
            G_ShakeController.instance.ShakeController(clamp, clamp, attackDuration);
            playerMove.modelAnimator.SetBool(rotationAttackAnimation, true);

            yield return new WaitForSeconds(attackDuration);

            playerMove.modelAnimator.SetBool(rotationAttackAnimation, false);
            playerMove.modelAnimator.SetBool(strageAnimation, false);
            swordCollider.enabled = false;
            
            yield return new WaitForSeconds(rotateEndTime);
            playerMove.isKinematic = false;

            swordObject.SetActive(false);
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        activate = true;
    }


    public void OnEndAttack()
    {
        playerMove.modelAnimator.SetBool(normalAttackAnimation, false);
    }

    public void AttackCollider(bool enabled)
    {
        swordCollider.enabled = enabled;
    }

    public void TriggerEnter(Collider other)
    {
        GameObject enemy = other.gameObject;
        if (!enemyHps.Keys.Contains(enemy))
        {
            enemyHps.Add(enemy, enemy.GetComponent<F_HP>());
        }
        enemyHps[enemy].OnDamage(playerParam.GetATK(), attackValue.attackValues[chain], Vector3.zero);
        //G_DamageEffectGenerator.instance.OnDamage(other.ClosestPoint(transform.position));
        S_SEManager.PlayRoseAttackHitSE(transform);
        chain = Mathf.Clamp(chain + 1, 0, attackValue.attackValues.Length-1);
        chainTime = 0f;
    }

    public void OnRoseSwordActiveSelf(bool activeSelf)
    {
        swordObject.SetActive(activeSelf);
    }

}
