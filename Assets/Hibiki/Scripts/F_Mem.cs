using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;

[RequireComponent(typeof(F_HP))]
[RequireComponent(typeof(Rigidbody))]
public class F_Mem : F_Enemy
{
    [Header("Absolute")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] int woolLayer = 1;
    [SerializeField] Material normalMaterial, angryMaterial, killedMaterial;


    [Header("Variable")]
    [SerializeField] float minSqrEscapeDistance = 100f;
    [SerializeField] float maxSqrEscapeDistance = 900f;
    [SerializeField] float walkSpeed=5f,runSpeed = 10f;
    [SerializeField] float rotateSpeed = 10f;

    [SerializeField] float escapeRadius = 10f;

    [SerializeField] float escapeTime = 5f;

    [SerializeField] float sqrAttackDistance = 2f;

    [SerializeField] float attackSpan;

    [SerializeField] float attackRadius;

    [SerializeField] float searchRadius;
    [SerializeField] float searchInterval;
    private float searchTime;
    private Vector3 point;

    [SerializeField] float materialChangeDuration = 0.5f;

    [SerializeField] Transform attackPoint;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] string isKilledBlend, isWalkAnim, onAttackAnim, onDamageAnim, onKillAnim;


    private float attackTime;
    private bool isEscape;
    private HanadayoRigidobody rb;
    private Vector3 velocity;
    [HideInInspector] public Transform player;
    private bool isAngry;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        hp = GetComponent<F_HP>();
        rb = GetComponent<HanadayoRigidobody>();

        isAngry = false;
        isEscape = false;
        searchTime = Random.Range(0f,searchInterval);
        attackTime = attackSpan;
        velocity = Vector3.zero;
        point = transform.position;
        animator.SetFloat(isKilledBlend, 0f);



        meshRenderer.materials[woolLayer] = normalMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.isKnockBack) return;
        if (hp.isKilled)
        {
            return;
        }

        if (isAngry)
        {
            agent.SetDestination(player.transform.position);
            float sqrMagitude = Vector3.SqrMagnitude(player.position - transform.position);

            if (sqrMagitude <= sqrAttackDistance)
            {
                animator.SetBool(isWalkAnim, false);
                Vector3 moveDirection = player.position - transform.position;
                moveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed * Time.deltaTime);
                attackTime -= Time.deltaTime;
                if (attackTime <= 0f)
                {
                    attackTime = attackSpan;
                    animator.SetTrigger(onAttackAnim);
                }
            }
            else
            {
                animator.SetBool(isWalkAnim, true);

                Vector3 moveDirection = MoveDirecton() - transform.position;
                moveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed * Time.deltaTime);
                velocity = transform.forward * runSpeed;
            }
        }
        else if(!isEscape)
        {
            if(searchTime >= searchInterval)
            {
                searchTime = 0f;
                var direction = searchRadius * Random.insideUnitCircle;
                point = transform.position + new Vector3(direction.x, 0f, direction.y);
            }
            Vector3 moveDirection = point - transform.position;
            moveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            searchTime += Time.deltaTime;
            if (moveDirection.sqrMagnitude <= 0.1f)
            {
                return;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed * Time.deltaTime);
            velocity = transform.forward * walkSpeed;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Attack()
    {
        var col = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer).Where(col => col.gameObject.CompareTag("Player")).ToArray();
        if (col.Length > 0)
        {
            col[0].GetComponent<F_HP>().OnDamage(hp.enemyPram.GetATK(), attackValue.attackValues[0], HanadayoRigidobody.GetPushAxis(transform.position, col[0].transform.position));
        }
    }

    private Vector3 MoveDirecton()
    {
        if (!player)
        {
            player = GameObject.Find("Player").transform;
        }

        agent.SetDestination(player.transform.position);

        foreach (var pos in agent.path.corners)
        {
            var diff2d = new Vector2(
                Mathf.Abs(pos.x - transform.position.x),
                Mathf.Abs(pos.z - transform.position.z)
            );

            if (0.1f <= diff2d.magnitude)
            {
                return pos;
            }
        }
        return player.position;
    }

    private void Move()
    {
        if(rb.onGround && !rb.isKnockBack)
        {
            rb.GetRigidbody().velocity = new Vector3(velocity.x, rb.GetRigidbody().velocity.y, velocity.z);
            velocity = Vector3.zero;
        }
    }

    public override void OnDamaged()
    {
        base.OnDamaged();
        if (!player)
        {
            player = GameObject.Find("Player").transform;
        }
        if (!isAngry)
        {
            isAngry = true;
            Material nowMaterial = meshRenderer.materials[woolLayer];
            DOVirtual.Float(0f, 1f, materialChangeDuration, (value) =>
            {
                meshRenderer.materials[woolLayer].Lerp(nowMaterial, angryMaterial, value);
            });
        }
        Vector3 direction = player.transform.position - transform.position;
        Quaternion look = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = look;

        animator.SetTrigger(onDamageAnim);

        NotifyDamaged();
    }

    public override void OnKilled()
    {
        base.OnKilled();
        G_FlowerEnergyReceiver receiver = G_FlowerEnergyReceiver.instance;


        rb.GetRigidbody().velocity = new Vector3(0f, rb.GetRigidbody().velocity.y, 0f);
        Material nowMaterial = meshRenderer.materials[woolLayer];
        DOVirtual.Float(0f, 1f, materialChangeDuration, (value) =>
        {
            meshRenderer.materials[woolLayer].Lerp(nowMaterial, killedMaterial, value);
        });

        animator.SetFloat(isKilledBlend, 1f);
        animator.SetTrigger(onKillAnim);
    }

    /// <summary>
    /// Mem tells the Mems around it that it has been damaged.
    /// </summary>
    private void NotifyDamaged()
    {
        var cols = Physics.OverlapSphere(transform.position, escapeRadius,enemyLayer).Where(col => col.gameObject.CompareTag("Enemy"));
        foreach (var col in cols)
        {
            if (col.gameObject == gameObject)
            {
                continue;
            }

            F_Mem mem = col.GetComponent<F_Mem>();
            if (mem)
            {
                mem.OnCompanionDamaged(transform.position);
                if (!player)
                {
                    mem.player = player;
                }
            }
        }
    }

    /// <summary>
    /// Triggered when a nearby Mem is damaged.
    /// </summary>
    public void OnCompanionDamaged(Vector3 damagePoint)
    {
        if (isAngry) { return; }
        StartCoroutine(escape());

        IEnumerator escape()
        {
            animator.SetBool(isWalkAnim, true);
            isEscape = true;
            float escapeDistance = Random.Range(minSqrEscapeDistance, maxSqrEscapeDistance);
            float escapingTime = 0f;

            while ((Vector3.SqrMagnitude(damagePoint - transform.position) <= escapeDistance)&&(escapingTime<escapeTime))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - damagePoint), Time.fixedDeltaTime * rotateSpeed);

                Vector3 velocity = transform.forward;
                velocity = new Vector3(velocity.x, 0f, velocity.z).normalized * runSpeed;
                rb.GetRigidbody().velocity = new Vector3(velocity.x, rb.GetRigidbody().velocity.y, velocity.z);

                escapingTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            animator.SetBool(isWalkAnim, false);
            isEscape = false;
        }
    }
}
