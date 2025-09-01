using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(F_HP))]
[RequireComponent(typeof(Rigidbody))]
public class F_Crows : F_Enemy
{
    //About crows attack  by crow types
    public enum AttackType
    {
        BlackType,
        PurpleType
    }

    [Header("??????")]
    [SerializeField] public AttackType attackType;

    [Header("????????")]
    [SerializeField] private float crowSpeed = 10.0f;
    private float distance;
    [SerializeField] protected Transform[] lapsPoint;
    int index = 0;
    float elapsed = 0f;

    [Header("????")]
    [SerializeField] RushRecklessly rushRecklessly;
    [SerializeField] EvilSplit evilSplit;

    [Header("????")]
    [SerializeField] ActionFuzzies actionFuzzies;
    [SerializeField] MoveFuzzies moveFuzzies;
    [Tooltip("????")]
    [SerializeField, Range(0,100)] int maxStamina;
    private float stamina;
    [SerializeField] float staminaSpeed = 3.5f;

    [SerializeField] Vector2 playerDistanceValue;
    // About Fly
    private bool flying;
    // About Roost
    [Tooltip("?????")]
    [SerializeField, Range(1, 8)] float minTime, maxTime;
    private bool roosting;
    [SerializeField] float flightDuration = 2.0f;
    float landingElapsed;

    [SerializeField] private Transform player;
    private Vector3 moveVelocity;
    private Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] Searcher searcher;
    [SerializeField] string onAttack, onLanding, onTakeOff, rise;
    [Header("??")]
    private bool attacked;
    [SerializeField] Transform attackTransform;
    private F_HP playerHP;
    [SerializeField] F_Param paramator;

#if UNITY_EDITOR
    [SerializeField, Range(0f, 1f)] float fuzzyTestStamina, fuzzyTestDistance;
    [SerializeField] string actionResult;

    private void OnValidate()
    {
        actionResult = actionFuzzies.GetMostFuzzyName(fuzzyTestStamina);
    }

#endif

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        hp = GetComponent<F_HP>();
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;
        flying = true;
        roosting = false;
        attacked = false;
        moveVelocity = Vector3.zero;
        StartCoroutine(IFlying());
    }

    private void Update()
    {
        if (hp.isKilled)
        {
            return;
        }
    }
    private void FixedUpdate()
    {
        OnMove();
    }

    public override void OnDamaged()
    {
        base.OnDamaged();
    }

    public void OnAtttck()
    {
        StopAllCoroutines();
        StartCoroutine(IRushRecklessly());
    }

    public void OnLoss()
    {

        StartCoroutine(IFlying());
    }

    public override void OnKilled()
    {
        base.OnKilled();
        StopAllCoroutines();
        Destroy(searcher);
    }

    public IEnumerator IFlying()
    {
        while (!hp.isKilled)
        {
            yield return StartCoroutine("I" + GetActionName());
        }
    }


    [Serializable]
    private class ActionFuzzies
    {
        public F_Enemy.FuzzyGrade fly, roost;

        public string GetMostFuzzyName(float staminaPercent)
        {
            var fuzzies = F_Enemy.FuzzyGrade.FuzziesToArray(roost,fly);
            return fuzzies.OrderBy(fuzzy => fuzzy.GetEvaluate(staminaPercent)).FirstOrDefault().name;
        }
    }

    public float GetStaminaPercent()
    {
        return stamina / maxStamina;
    }

    private float StaminaDeal
    {
        get => stamina;
        set
        {
            if (maxStamina < value)
                stamina = maxStamina;
            else if (value > stamina)
                stamina = value;
            else
                stamina = 0;
        }
    }

    private string GetActionName()
    {
        float staminaPercent = GetStaminaPercent();
        //float playerDistance = Mathf.InverseLerp(playerDistanceValue.x, playerDistanceValue.y, Vector3.Magnitude(player.position - transform.position));
        return actionFuzzies.GetMostFuzzyName(staminaPercent);
    }

    public IEnumerator IFly()
    {

        while (flying)
        {
            Debug.Log("Flying");
            elapsed = 0f;
            while (elapsed < 1.0f)
            {
                
                // ???????????
                Transform currentPoint = lapsPoint[index];
                Transform nextPoint = lapsPoint[(index + 1) % lapsPoint.Length];

                // ????????????????????
                distance = Vector3.Distance(currentPoint.position, nextPoint.position);

                stamina -= Time.deltaTime * staminaSpeed;
                Debug.Log(stamina);
                elapsed += Time.deltaTime * crowSpeed / distance;
                if ((nextPoint.position - currentPoint.position) != Vector3.zero)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(nextPoint.position - currentPoint.position, transform.up).eulerAngles.y, 0f), elapsed);
                }
                yield return null;
                transform.position = Vector3.Lerp(currentPoint.position, nextPoint.position, elapsed);
            }
            index = (index + 1) % lapsPoint.Length;
            if (GetStaminaPercent() < 0.5) yield return StartCoroutine(IFlying());
        }
    }

    public IEnumerator IRoost()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);

        animator.SetBool(onLanding, true);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        
        Debug.Log("Roosting");
        flying = false;
        var roostTime = UnityEngine.Random.Range(minTime, maxTime);
        roosting = true;
        float count = 0f;
        float _stamina = 0f;
        float landingSpeed = 0.0001f;
        Vector3 originPoint = ray.origin;

        while(roosting)
        {
            if (hit.collider != null)
            {
                landingElapsed = 0f;
                Vector3 landingpPosition = new Vector3(transform.position.x, hit.point.y + 1f, transform.position.z);
                
                while (landingElapsed < 0.1f)
                {
                    
                    float time = landingElapsed > 0.0f ? Time.deltaTime * landingSpeed : 0.1f;
                    landingElapsed += Mathf.Clamp01(time);
                    transform.position = Vector3.Lerp(transform.position, landingpPosition, landingElapsed);
                }
            }
            yield return null;
            count += Time.fixedDeltaTime * 2.0f/roostTime;
            if (count < roostTime)
            {
                _stamina += Time.fixedDeltaTime * 3.0f;
            }
            else
            {
                yield return new WaitForSeconds(flightDuration);
                roosting = false;
                animator.SetBool(onLanding, false);
                flying = true;
                roostTime = 0f;
                count = 0f;
            }
        }
        StaminaDeal = stamina + _stamina;
        stamina = StaminaDeal;
        yield return null;
        animator.SetBool(onTakeOff, true);
        animator.SetFloat(rise, Time.deltaTime * 0.5f);
        while (landingElapsed < 0.1f)
        {
            //landingElapsed += Time.deltaTime;
            float time = landingElapsed > 0.0f ? Time.deltaTime * landingSpeed : 0.1f;
            landingElapsed += Mathf.Clamp01(time);
            transform.position = Vector3.Lerp(transform.position, originPoint, landingElapsed);
        }
        animator.SetBool(onTakeOff, false);
        yield return new WaitForSeconds(flightDuration);
        StopCoroutine(IRoost());

    }

    [Serializable]
    private class MoveFuzzies
    {
        public F_Enemy.FuzzyGrade foward, back;
        public string GetMostFuzzyName(float HPPercent, float playerDistancePercent)
        {
            var fuzzies = F_Enemy.FuzzyGrade.FuzziesToArray(foward, back);
            return fuzzies.OrderByDescending(fuzzy => fuzzy.GetEvaluate(HPPercent, playerDistancePercent)).FirstOrDefault().name;
        }

    }


    //The following is related to Move and Act
    //BlackCrowAttack
    [Serializable]
    private class RushRecklessly
    {
        [SerializeField] public float speed = 20.0f;
        
    }

    public IEnumerator IRushRecklessly()
    {
        Transform player = searcher.player;
        attacked = false;

        while (searcher.isPunish)
        {

            for (float t = 0; t < 1; t += Time.fixedDeltaTime)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(player.position - transform.position, transform.up).eulerAngles.y, 0f), Time.fixedDeltaTime * 10f);
            }
            yield return new WaitForSeconds(2f);
            animator.SetBool(onAttack, true);
            for (float f = 0; f < 1; f += Time.fixedDeltaTime)
            {
                moveVelocity = transform.forward * rushRecklessly.speed;
                if (!attacked)
                {
                    Collider[] col = Physics.OverlapBox(attackTransform.position, attackTransform.localScale, attackTransform.rotation).Where(value => value.gameObject.CompareTag("Player")).ToArray();
                    if (col.Length != 0)
                    {
                        if (!playerHP) playerHP = col[0].gameObject.GetComponent<F_HP>();
                        playerHP.OnDamage(paramator.GetATK(), attackValue.attackValues[0], Vector3.zero);
                        attacked = true;
                    }
                }
                yield return null;
                animator.SetBool(onAttack, false);
            }
        }
    }

    public void OnMove()
    {
        Vector3 axis = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (axis != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(axis, transform.up).eulerAngles.y, 0f), Time.fixedDeltaTime * 10f);
        }
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        moveVelocity = Vector3.zero;
    }

    //PurpleCrowAttack
    [Serializable]
    private class EvilSplit
    {
        public float chantingTime;
        public float minWaitTime, maxWaitTime;
        public float GetWaitTime()
        {
            return UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        }

        public GameObject prefab;
        public float summonSpan;
        public int amount = 1;
    }

    public IEnumerator IEvilSplit()
    {
        yield return new WaitForSeconds(evilSplit.chantingTime);
        yield return null;
    }

    /*
    protected F_HP GetHP()
    {
        if (!hp)
        {
            hp = GetComponent<F_HP>();
        }
        return hp;
    }

    private string GetMoveName()
    {
        float hpPercet = GetHP().GetHPPercent();
        float playerDistance = Mathf.InverseLerp(playerDistanceValue.x, playerDistanceValue.y, Vector3.Magnitude(player.position - transform.position));
        return moveFuzzies.GetMostFuzzyName(hpPercet, playerDistance);
    }

    //Crows battle moving
    private IEnumerator IWait(float waitTime)
    {
        switch (GetMoveName())
        {
            case "MoveFoward":
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    yield return StartCoroutine(Foward(direction, waitTime));
                    break;
                }
            case "MoveBack":
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    yield return StartCoroutine(Back(direction, waitTime));
                    break;
                }
            default:
                {
                    yield return new WaitForSeconds(waitTime);
                    break;
                }
        }
    }

    private IEnumerator Foward(Vector3 direction, float duration)
    {
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator Back(Vector3 direction, float duration)
    {
        yield return new WaitForFixedUpdate();
    }*/
}
