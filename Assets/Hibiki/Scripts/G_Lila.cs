using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class G_Lila : G_BossBase
{
    [SerializeField] CanvasGroup hpBar;
    [Header("モデル座標")]
    [SerializeField] Transform leftArm, rightArm;
    [Header("技")]
    [SerializeField] PoisonicLake poisonicLake;
    [SerializeField] Crow crow;
    [SerializeField] ElThunder elThunder;
    [SerializeField] EvilSplit evilSplit;
    [SerializeField] ThunderBurst thunderBurst;
    [SerializeField] AlkaloidEnd alkaloidEnd;

    [Header("技選択")]
    [SerializeField] AttackFuzzies attackFuzzies;
    [SerializeField] MoveFuzzies moveFuzzies;
    [SerializeField] Vector2 playerDistanceValue;

#if UNITY_EDITOR
    [SerializeField, Range(0f, 1f)] float fuzzyTestHP, fuzzyTestDistance;
    [SerializeField] string attackResult, moveResult;

    void OnValidate()
    {
        attackResult = attackFuzzies.GetMostFuzzyName(fuzzyTestHP, fuzzyTestDistance);
        moveResult = moveFuzzies.GetMostFuzzyName(fuzzyTestHP, fuzzyTestDistance);
    }

#endif

    [Serializable]
    private class AttackFuzzies
    {
        public F_Enemy.FuzzyGrade poisonicLake, summonCrows, elThunder, evilSplit, thunderBurst;

        public string GetMostFuzzyName(float HPPercent, float playerDistancePercent)
        {
            var fuzzies = F_Enemy.FuzzyGrade.FuzziesToArray(poisonicLake, summonCrows, elThunder, evilSplit, thunderBurst);
            return fuzzies.OrderByDescending(fuzzy => fuzzy.GetEvaluate(HPPercent, playerDistancePercent)).FirstOrDefault().name;
        }
    }
    [Serializable]
    private class MoveFuzzies
    {
        public F_Enemy.FuzzyGrade moveFoward, moveBack, warp;

        public string GetMostFuzzyName(float HPPercent, float playerDistancePercent)
        {
            var fuzzies = F_Enemy.FuzzyGrade.FuzziesToArray(moveFoward, moveBack, warp);
            return fuzzies.OrderByDescending(fuzzy => fuzzy.GetEvaluate(HPPercent, playerDistancePercent)).FirstOrDefault().name;
        }
    }
    [Serializable]
    private class PoisonicLake : Magic
    {
        public GameObject attackPrefab;
        public GameObject magicCirclePrefab;
        public float summonSpan = 0.25f;
        public float summonRadius = 20f;
        public float summonHeight = 6.5f;
        public int amount = 3;
    }

    [Serializable]
    private class Crow : Magic
    {
        public GameObject crowPrefab;
        public GameObject summonAreaPrefab;
        public Vector3 summonAreaLocalPoint = new Vector3(0f, 5f, -10f);
        public float waidingTime = 1.5f;
        public float summonAreaRadius = 10f;
        public float summonAreaSpreadDuration = 0.25f;
        public int amount = 5;
    }
    [Serializable]
    private class ElThunder : Magic
    {
        public GameObject prefab;
        public float lifeTime = 0.7f;
        public float chantingRotateSpeed = 5f;
        public float summonSpan = 0.2f;
        public float rotateAngle = 3f;
        public float eachDistance = 0.25f;
        public int amount = 5;
    }
    [Serializable]
    private class EvilSplit : Magic
    {
        public GameObject prefab;
        public float lifeTime = 3f;
        public float summonSpan = 0.7f;
        public float chantingRotateSpeed = 5f;
        public float attackingRotateSpeed = 10f;
        public int amount = 5;
    }
    [Serializable]
    private class ThunderBurst : Magic
    {
        public GameObject chargePrefab;
        public GameObject burstPrefab;
        public float radius = 3f;
        public float shakeCameraPower = 1f;
        public float beforeBurstTime = 0.2f;
    }
    [Serializable]
    private class AlkaloidEnd
    {
        public GameObject prefab;
        public Vector3 summonPosition = new Vector3(0f, 100f, 0f);
        public float chantingTime;
        public float spreadCloudDuration = 3f;
        public float activateDuration = 10f;
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    public override void OnBattleStart()
    {
        base.OnBattleStart();
        StartCoroutine(Battle());
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(AlkaloidEnd());
        //}
    }
    public IEnumerator Battle()
    {
        while (!GetHP().isKilled)
        {
            yield return StartCoroutine("I" + GetAttackName());
        }
    }

    private string GetAttackName()
    {
        float hpPercet = GetHP().GetHPPercent();
        float playerDistance = Mathf.InverseLerp(playerDistanceValue.x, playerDistanceValue.y, Vector3.Magnitude(player.position - transform.position));
        return attackFuzzies.GetMostFuzzyName(hpPercet, playerDistance);
    }
    private string GetMoveName()
    {
        float hpPercet = GetHP().GetHPPercent();
        float playerDistance = Mathf.InverseLerp(playerDistanceValue.x, playerDistanceValue.y, Vector3.Magnitude(player.position - transform.position));
        return moveFuzzies.GetMostFuzzyName(hpPercet, playerDistance);
    }

    public IEnumerator IPoisonicLake()
    {
        Vector3 summonCenter = new Vector3(transform.position.x, transform.position.y + poisonicLake.summonHeight, transform.position.z);
        S_SEManager.PlayLilaChantingPoisonLakeSE(transform);
        StartCoroutine(Chanting(poisonicLake.chantingTime, "chantingPoisonLake"));
        Transform _magicCircle = Instantiate(poisonicLake.magicCirclePrefab, summonCenter, Quaternion.Euler(180f, 0f, 0f)).transform;
        S_SEManager.PlayLilaMagicCircleSE(_magicCircle);
        _magicCircle.localScale = Vector3.zero;
        _magicCircle.DOScale(2f * poisonicLake.summonRadius, poisonicLake.chantingTime).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(poisonicLake.chantingTime);
        for(int i = 0; i < poisonicLake.amount; i++)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * poisonicLake.summonRadius;
            Instantiate(poisonicLake.attackPrefab, new Vector3(point.x, 0f, point.y + 0f) + summonCenter, Quaternion.identity);
            yield return new WaitForSeconds(poisonicLake.summonSpan);
        }
        _magicCircle.DOScale(0f, poisonicLake.afterAttackWait).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(poisonicLake.afterAttackWait);
        Destroy(_magicCircle.gameObject);
        yield return StartCoroutine(IWait(poisonicLake.GetWaitTime()));
    }
    public IEnumerator ICrow()
    {
        S_SEManager.PlayLilaChantingBardSE(transform);
        StartCoroutine(Chanting(crow.chantingTime + crow.waidingTime, "lila_bard"));
        yield return new WaitForSeconds(crow.chantingTime);
        Vector3 _crowSummonArea = transform.TransformDirection(crow.summonAreaLocalPoint);
        Transform summonArea = Instantiate(crow.summonAreaPrefab, transform.position + _crowSummonArea, Quaternion.identity).transform;
        summonArea.localScale = Vector3.zero;
        summonArea.DOScale(crow.summonAreaRadius, crow.summonAreaSpreadDuration).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(crow.summonAreaSpreadDuration);
        List<G_DependentCrow> crows = new List<G_DependentCrow>();
        for(int i = 0; i < crow.amount; i++)
        {
            Quaternion crowQuaternion = Quaternion.Euler(UnityEngine.Random.Range(-45f, 0f), UnityEngine.Random.Range(0f, 360f), 0f);
            Transform crowTransform = Instantiate(crow.crowPrefab, summonArea.position, crowQuaternion).transform;
            //S_SEManager.PlayLila_BardAttackSE(crow);
            crowTransform.position += crowTransform.forward * crow.summonAreaRadius / 2f;
            crows.Add(crowTransform.GetComponent<G_DependentCrow>());
            crows[i].Setting(player);
        }
        yield return new WaitForSeconds(crow.waidingTime);
        summonArea.DOScale(0f, 0.5f).SetEase(Ease.InCirc).OnComplete(()=>Destroy(summonArea.gameObject));
        foreach(var c in crows)
        {
            c.goAttack = true;
        }
        yield return new WaitForSeconds(crow.afterAttackWait);
        yield return StartCoroutine(IWait(crow.GetWaitTime()));
    }
    public IEnumerator IElThunder()
    {
        S_SEManager.PlayLilaChantingLThunderSE(transform);
        StartCoroutine(Chanting(elThunder.chantingTime, "chantingLthunder"));
        float span = elThunder.chantingTime / (float)elThunder.amount;
        List<G_EvilSplit> elThunders = new List<G_EvilSplit>();
        
        float rot = ((float)elThunder.amount - 1) / 2f;
        float startX = -elThunder.eachDistance * (elThunder.amount - 1) * 0.5f;
        for (int i = 0; i < elThunder.amount; i++)
        {
            GameObject obj = Instantiate(elThunder.prefab, transform.position + transform.right * (startX + (i * elThunder.eachDistance)) + transform.up, Quaternion.identity);
            Vector3 size = obj.transform.localScale;
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(size, span).SetEase(Ease.OutSine);
            elThunders.Add(obj.GetComponent<G_EvilSplit>());
            for (float f = 0; f < span; f += Time.deltaTime)
            {
                var aim = Quaternion.LookRotation((player.position - transform.position), transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, aim.eulerAngles.y, 0f), Time.deltaTime * elThunder.chantingRotateSpeed);
                yield return null;
            }
        }
        for (int i = 0; i < elThunder.amount; i++)
        {
            Destroy(elThunders[i].gameObject,elThunder.lifeTime);
            elThunders[i].transform.rotation = Quaternion.LookRotation(player.position - (transform.position + transform.up), transform.up) * Quaternion.Euler(0f, elThunder.rotateAngle * (i - rot), 0f);
            elThunders[i].isGo = true;
            S_SEManager.PlayLila_L_thunderSE(transform);
            yield return new WaitForSeconds(elThunder.summonSpan);
        }
        yield return new WaitForSeconds(elThunder.afterAttackWait);
        yield return StartCoroutine(IWait(elThunder.GetWaitTime()));

    }
    public IEnumerator IEvilSplit()
    {
        S_SEManager.PlayLilaChantingShockWaveSE(transform);
        StartCoroutine(Chanting(evilSplit.chantingTime, "chantingPoisonLake"));
        for (float f = 0; f < evilSplit.chantingTime; f += Time.deltaTime)
        {
            var aim = Quaternion.LookRotation((player.position - transform.position), transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, aim.eulerAngles.y, 0f), Time.deltaTime * evilSplit.chantingRotateSpeed);
            yield return null;
        }
        for (int i = 0; i < evilSplit.amount; i++)
        {
            Destroy(Instantiate(evilSplit.prefab, leftArm.position, Quaternion.LookRotation(player.position - leftArm.position, transform.up)), evilSplit.lifeTime);
            S_SEManager.PlayLilaShockWaveSE(transform);
            for (float f = 0; f < evilSplit.summonSpan; f += Time.deltaTime)
            {
                var aim = Quaternion.LookRotation((player.position - transform.position), transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, aim.eulerAngles.y, 0f), Time.deltaTime * evilSplit.attackingRotateSpeed);
                yield return null;
            }
        }
        yield return new WaitForSeconds(evilSplit.afterAttackWait);
        yield return StartCoroutine(IWait(evilSplit.GetWaitTime()));

    }

    public IEnumerator IThunderBurst()
    {
        S_SEManager.PlayLilaChantingBom1SE(transform);
        StartCoroutine(Chanting(thunderBurst.chantingTime, "lila_bom1"));
        Transform chargeTransform = Instantiate(thunderBurst.chargePrefab, transform.position, Quaternion.identity).transform;
        chargeTransform.localScale = Vector3.zero;
        for(float f = 0f; f < 1f; f += Time.deltaTime / thunderBurst.chantingTime)
        {
            chargeTransform.localScale = thunderBurst.radius * f * Vector3.one;
            yield return null;
        }
        Destroy(chargeTransform.gameObject);

        S_SEManager.PlayLilaChantingBom2SE(transform);
        yield return new WaitForSeconds(thunderBurst.beforeBurstTime);
        Instantiate(thunderBurst.burstPrefab, transform.position, Quaternion.identity).transform.localScale = thunderBurst.radius * Vector3.one;
        S_SEManager.PlayLilaBomSE(transform);
        G_ShakeCamera.instance.ShakeCamera(thunderBurst.shakeCameraPower);

        yield return new WaitForSeconds(thunderBurst.afterAttackWait);
        yield return StartCoroutine(WarpToGround());
    }

    private IEnumerator IWait(float waitTime)
    {
        switch (GetMoveName())
        {
            case "MoveFoward":
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    yield return StartCoroutine(MoveFoward(direction, waitTime));
                    break;
                }
            case "MoveBack":
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    yield return StartCoroutine(MoveBack(direction, waitTime));
                    break;
                }
            case "Warp":
                {
                    yield return StartCoroutine(WarpToGround());
                    yield return new WaitForSeconds(waitTime);
                    break;
                }
            default:
                {
                    yield return new WaitForSeconds(waitTime);
                    break;
                }
        }
    }
    private IEnumerator Chanting(float duration,string chantingSEName)
    {
        anim.animator.SetBool("IsChanting", true);
        yield return new WaitForSeconds(duration);
        anim.animator.SetBool("IsChanting", false);
        S_SEManager.SEStop(chantingSEName);
        anim.animator.SetTrigger("OnBurst");
    }

    public IEnumerator IAlkaloidEnd()
    {
        yield return new WaitForSeconds(alkaloidEnd.chantingTime);
        GameObject cloud = Instantiate(alkaloidEnd.prefab, alkaloidEnd.summonPosition, Quaternion.identity);
        cloud.transform.localScale = Vector3.zero;
        cloud.transform.DOScale(1f, alkaloidEnd.spreadCloudDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(alkaloidEnd.spreadCloudDuration);
        yield return new WaitForSeconds(alkaloidEnd.activateDuration);
    }

    public override void OnKilled()
    {
        base.OnKilled();
        hpBar.DOFade(0f, 1f);
    }
}
