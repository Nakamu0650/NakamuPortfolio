using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;
using EggSystem;
using DG.Tweening;

public class P_PlayerMove : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;//方向を指定
    [SerializeField] LayerMask groundLayer;//設置判定をとるレイヤーを指定
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector2 moveAxis;//コントローラーの向きを保存
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool isKinematic;

    
    private bool overWritePlayerPosition;
    private Vector3 overWriteForward;
    public bool canJump;
    
    private Vector3 moveAmount;
    private Quaternion playerMoveDirection;
    private RaycastHit hitGroundPoint;
    private Vector3 startPos;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isFlying;

    private float consecutiveAvoidancePenaltyResetTime = 0f;
    private int consecutiveAvoidanceCount = 0;
    private bool isAvoiding;

    [SerializeField] Controller controller;
    [SerializeField] Move move;
    [SerializeField] Fly fly;
    [SerializeField] Jump jump;
    [SerializeField] Avoid avoid;
    [SerializeField] Slip slip;
    public List<Transform> returnPoints;
    [SerializeField] float returnSpeed = 10f;
    [Space()]
    [SerializeField]public Animator modelAnimator;
    [SerializeField] Animations animations;
    
    public string attackSpeedAnimation;
    [SerializeField] GameObject umbrellaObj;

    public Events events;
    private float inAirAnimationBlendValue = 0f;
    private bool isSlow;
    private bool isReturning;
    

    private bool isSlip;
    private float slipTime;
    private float waitTime;

    private bool waitingJump;

    private F_HP hp;
    private P_Metamorphosis metamorphosis;
    [HideInInspector]public G_BuffSystem buffSystem;

    public static P_PlayerMove instance;
    public bool isMove;

    [Serializable]
    public class Controller
    {
        [Range(0f, 1f)] public float walkThreshold = 0.1f, runThreshold = 0.5f;
    }
    [Serializable]
    public class Move
    {
        public float speed = 10f;
        [Range(0f, 1f)] public float walkMultiply = 0.25f;
        public float rotateSpeed = 10f;
    }
    [Serializable]
    public class Fly
    {
        public float speed = 15f;
        public float aerodynamicDrag = 3f;
        public float minDistance = 3.5f;
    }
    [Serializable]
    public class Jump
    {
        public float power = 50;
        public float upDacay = 10f;
        public float downDeacay = 6f;
        public float maxFallSpeed = 20f;
        public float rayLength = 0.2f;
    }
    [Serializable]
    public class Avoid
    {
        public Gaps gaps = new Gaps(0.05f, 0.25f, 0.15f);
        public float speed = 20f;
        public List<float> consecutiveDelaies = new List<float>();
        public float delayResetDuration = 1.5f;
    }
    [Serializable]
    public class Slip
    {
        public float speed = 7f;
        public Range<float> slopeThreshold = new Range<float>(20f, 30f);
        public Range<float> slipThreshold = new Range<float>(20f, 29f);
    }
    [Serializable]
    public class Animations
    {

        [Header("Trigger")]
        public string onAvoid;
        public string onJump;
        [Header("Boolean")]
        public string isAir;
        public string isGliding;
        [Header("Float")]
        public string moveState;
        public string moveSpeed;
        public string avoidSpeed;
        public string onWait;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hp = GetComponent<F_HP>();
        buffSystem = GetComponent<G_BuffSystem>();
        metamorphosis = P_Metamorphosis.instance;
        isJumping = false;
        waitingJump = false;
        isFlying = false;
        isSlip = false;
        isAvoiding = false;
        isSlow = false;
        isMove = true;
        canMove = true;
        canJump = true;
        hp.isInvincible = false;
        isReturning = false;
        overWritePlayerPosition = false;
        slipTime = 0f;
        consecutiveAvoidancePenaltyResetTime = 0f;
        consecutiveAvoidanceCount = 0;
        inAirAnimationBlendValue = 0f;
        overWriteForward = Vector3.zero;
        startPos = transform.position;
        hitGroundPoint = new RaycastHit();
        returnPoints = new List<Transform>();
    }


    void FixedUpdate()//Processes related to Rigidbody
    {
        //Player position change
        if (isKinematic)
        {
            SetMoveAnimation(0);
            modelAnimator.SetFloat(animations.moveSpeed, 1f);
            rb.velocity = Vector3.zero;
            return;
        }

        if (canMove)
        {
            PlayerPosition();
        }
        else
        {
            SetMoveAnimation(0);
            modelAnimator.SetFloat(animations.moveSpeed, 1f);
        }

        //Jump Script
        if (!isFlying)
        {
            if (rb.velocity.y > 0)//Increased downward energy when rising
            {
                float subtractValue = rb.velocity.y * jump.upDacay * Time.fixedDeltaTime;
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - subtractValue, rb.velocity.z);
                inAirAnimationBlendValue = 0f;
            }
            else if (rb.velocity.y < 0)//Increased downward energy during descent
            {
                float subtractValue = rb.velocity.y * jump.downDeacay * Time.fixedDeltaTime;
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y + subtractValue, -jump.maxFallSpeed, 0), rb.velocity.z);
                inAirAnimationBlendValue = Mathf.Lerp(inAirAnimationBlendValue, rb.velocity.y == -jump.maxFallSpeed ? 1f : 0f, Time.fixedDeltaTime * 10f);
            }
        }


    }
    private void Update()
    {
        bool getIsJumping = GetIsJumping();
        if (isJumping && !getIsJumping)
        {
            isJumping = false;
            modelAnimator.SetBool(animations.isAir, false);
            isFlying = false;
            //modelAnimator.SetBool(animations.isGliding, isFlying);
            umbrellaObj.SetActive(isFlying);
            rb.drag = 0;
            S_SEManager.PlayPlayerLandingSE(transform);
            S_SEManager.SEStop("playerfly");
        }
        if (!isJumping && getIsJumping)
        {
            isJumping = true;
            modelAnimator.SetBool(animations.isAir, true);
        }
        if (consecutiveAvoidancePenaltyResetTime > 0f)
        {
            consecutiveAvoidancePenaltyResetTime -= Time.deltaTime;
            if (consecutiveAvoidancePenaltyResetTime <= 0f)
            {
                consecutiveAvoidanceCount = 0;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)//Get controller movement input
    {
        if (isMove)
        {
            moveAxis = context.ReadValue<Vector2>();
        }
    }

    private void SetMoveAnimation(int num)
    {
        float value = Mathf.Lerp(modelAnimator.GetFloat(animations.moveState), (float)num, 10f * Time.deltaTime);
        modelAnimator.SetFloat(animations.moveState, value);
    }

    public void OnJump(InputAction.CallbackContext context)//Get controller jump input
    {
        if (waitingJump)
        {
            return;
        }
        if (!canJump)
        {
            return;
        }

        if (context.performed && GetCanMove())
        {
            if (!isJumping)
            {
                waitingJump = true;
                modelAnimator.SetTrigger(animations.onJump);
                rb.velocity += new Vector3(0, jump.power, 0);
                events.onJump.Invoke();
                S_SEManager.PlayPlayerJumpSE(transform);
                S_SEManager.SEStop("playerfly");
            }
            else
            {
                if (isFlying)
                {
                    isFlying = false;
                    events.onFlyEnd.Invoke();
                    S_SEManager.SEStop("playerfly");
                    S_SEManager.PlayPlayerUmbrellaPutAwaySE(transform);
                    rb.drag = 0;
                    metamorphosis.OpenUmbrella(true);
                }
                else if (IsFlyable())
                {
                    isFlying = true;
                    events.onFly.Invoke();
                    S_SEManager.PlayPlayerUmbrellaTakeOutSE(transform);
                    S_SEManager.PlayPlayerFlySE(transform);
                    rb.drag = fly.aerodynamicDrag;
                    metamorphosis.CloseUmbrella(true);
                }
                modelAnimator.SetBool(animations.isGliding, isFlying);
                metamorphosis.ChangeUmbrella(isFlying);
            }
        }
    }
    public void OnAvoid(InputAction.CallbackContext context)//Get controller avoid input
    {
        if (context.performed && !isAvoiding && !isJumping)
        {
            if (isMove)
            {
                isAvoiding = true;
                events.onAvoid.Invoke();
                StartCoroutine(AvoidCoroutine(consecutiveAvoidanceCount));
                consecutiveAvoidanceCount = Mathf.Clamp(consecutiveAvoidanceCount + 1, 0, avoid.consecutiveDelaies.Count - 1);
                consecutiveAvoidancePenaltyResetTime = avoid.delayResetDuration;
            }
        }
    }
    private IEnumerator AvoidCoroutine(int count)
    {
        Vector3 _forward = transform.forward;

        canMove = false;
        modelAnimator.SetTrigger(animations.onAvoid);
        S_SEManager.PlayPlayerAvoidSE(transform);
        umbrellaObj.SetActive(true);
        float _decelation = avoid.consecutiveDelaies[count];
        float allDuration = (avoid.gaps.before + avoid.gaps.after) * _decelation + avoid.gaps.affect;
        modelAnimator.SetFloat(animations.avoidSpeed, 1f / allDuration);
        //Gap when it occurs
        for (float f = 0; f < 1f; f += Time.fixedDeltaTime / (avoid.gaps.before * _decelation))
        {
            Vector3 _velocity = avoid.speed * f * _forward / _decelation;
            rb.velocity = new Vector3(_velocity.x, rb.velocity.y, _velocity.z);
            var slopeDeceleration = SlopeDeceleration(transform.forward);
            if (slopeDeceleration.slopeAngle > slip.slipThreshold.Max)
            {
                isSlip = true;
                isAvoiding = false;
                canMove = true;
                umbrellaObj.SetActive(false);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }

        //Invincible time
        hp.isInvincible = true;
        for (float f = 0; f < 1f; f += Time.fixedDeltaTime / avoid.gaps.affect)
        {
            Vector3 _velocity = _forward * avoid.speed;
            rb.velocity = new Vector3(_velocity.x, rb.velocity.y, _velocity.z);
            var slopeDeceleration = SlopeDeceleration(transform.forward);
            if (slopeDeceleration.slopeAngle > slip.slipThreshold.Max)
            {
                isSlip = true;
                isAvoiding = false;
                hp.isInvincible = false;
                canMove = true;
                umbrellaObj.SetActive(false);
                yield break;
            }
            if (isJumping)
            {
                isAvoiding = false;
                canMove = true;
                umbrellaObj.SetActive(false);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }

        //Gap that appears after occurrence
        hp.isInvincible = false;
        float afterSpeedPercent = move.speed / avoid.speed;
        for (float f = 1; f > afterSpeedPercent; f -= afterSpeedPercent * Time.fixedDeltaTime / (avoid.gaps.after * _decelation))
        {
            Vector3 _velocity = avoid.speed * f * _forward / _decelation;
            rb.velocity = new Vector3(_velocity.x, rb.velocity.y, _velocity.z);
            var slopeDeceleration = SlopeDeceleration(transform.forward);
            if (slopeDeceleration.slopeAngle > slip.slipThreshold.Max)
            {
                isSlip = true;
                isAvoiding = false;
                canMove = true;
                umbrellaObj.SetActive(false);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        canMove = true;
        isAvoiding = false;
        umbrellaObj.SetActive(false);
    }

    public bool GetCanMove()
    {
        if (!canMove)
        {
            Debug.Log("canMove");
            return false;
        }
        if (isAvoiding)
        {
            Debug.Log("isAvoiding");
            return false;
        }
        if (isReturning)
        {
            Debug.Log("isReturning");
            return false;
        }
        return true;
    }

    public bool GetCanAttack()
    {
        if (!canMove)
        {
            Debug.Log("canMove");
            return false;
        }
        if (isAvoiding)
        {
            Debug.Log("isAvoiding");
            return false;
        }
        if (isReturning)
        {
            Debug.Log("isReturning");
            return false;
        }
        if (isJumping)
        {
            Debug.Log("isJumping");
            return false;
        }
        if (isFlying)
        {
            Debug.Log("isFlying");
            return false;
        }
        return true;
    }

    public Vector3 PositioningFromAngle(float angle)//Control rotation with trigonometric functions
    {
        Vector3 vector = new Vector3(0, Mathf.Sin(Mathf.PI * angle / 180f), -Mathf.Cos(Mathf.PI * angle / 180f));
        return vector;
    }

    public void OverWritePlayerPosition(Vector3 _velocity)
    {
        overWritePlayerPosition = true;
        overWriteForward = _velocity;
    }

    private void PlayerPosition()//Update position
    {
        if (!isJumping || isFlying)
        {
            float moveSpeedSqr = moveAxis.sqrMagnitude;
            if (moveSpeedSqr >= controller.walkThreshold)
            {
                playerMoveDirection = Quaternion.identity;
                Quaternion cameraRotation = Quaternion.LookRotation(transform.position - cameraTransform.transform.position);
                playerMoveDirection.eulerAngles = new Vector3(0, Mathf.Atan2(moveAxis.x, moveAxis.y) * Mathf.Rad2Deg + cameraRotation.eulerAngles.y, 0);
            }
            else
            {
                playerMoveDirection = transform.rotation;
            }

            Vector3 forward = overWritePlayerPosition ? overWriteForward : transform.forward;

            var slopeDeceleration = SlopeDeceleration(forward);


            if (isSlip)
            {
                events.onSlip.Invoke();
                rb.velocity = (slopeDeceleration.tangent + new Vector3(0f, -10f, 0f)) * slip.speed * Mathf.Clamp01(slipTime);
                slipTime += Time.deltaTime * 4f;
                if (slopeDeceleration.slopeAngle < slip.slipThreshold.Min)
                {
                    isSlip = false;
                    slipTime = 0f;
                }
            }
            else
            {
                float moveAggreate = (isFlying ? fly.speed : move.speed) * slopeDeceleration.deceleration;

                moveAmount = forward;


                isSlow = (Input.GetKey(KeyCode.LeftShift) || (moveSpeedSqr < controller.runThreshold));
                if (isSlow) moveAmount *= move.walkMultiply;
                if (moveSpeedSqr < controller.walkThreshold) moveAmount = Vector3.zero;

                moveAmount *= buffSystem.GetValue(G_BuffSystem.BuffType.MoveSpeed);

                float animationSpeed = moveAmount.magnitude;

                moveAmount *= moveAggreate;

                transform.rotation = Quaternion.Lerp(transform.rotation, playerMoveDirection, Time.fixedDeltaTime * move.rotateSpeed);
                rb.velocity = new Vector3(moveAmount.x, rb.velocity.y, moveAmount.z);
                if ((animationSpeed < 0.1f))
                {
                    modelAnimator.SetFloat(animations.moveSpeed, 1f);
                    SetMoveAnimation(0);
                }
                else
                {
                    modelAnimator.SetFloat(animations.moveSpeed, animationSpeed * slopeDeceleration.deceleration);
                    SetMoveAnimation(isSlow ? 1 : 2);
                }
            }
            if (slopeDeceleration.slopeAngle > slip.slipThreshold.Max)
            {
                isSlip = true;
            }
            overWritePlayerPosition = false;


        }
    }

    public void ReturnToSafeGroundPoint()
    {
        Vector3 _startPoint = transform.position;
        Vector3 _endPoint = returnPoints.OrderBy(point => Vector3.SqrMagnitude(point.position - _startPoint)).FirstOrDefault().position;
        if (_endPoint == Vector3.up)
        {
            _endPoint = startPos;
        }
        float _returnDistance = Vector3.Magnitude(_endPoint - _startPoint);
        float _returnDuration = _returnDistance / returnSpeed;
        Vector3 _lerpPoint = Vector3.Lerp(_endPoint, _startPoint, 0.5f);
        Vector3 _beziePoint = new Vector3(_lerpPoint.x, Mathf.Max(_endPoint.y, _startPoint.y) + _returnDistance, _lerpPoint.z);

        StartCoroutine(safeGroundEnumerator());

        IEnumerator safeGroundEnumerator()
        {
            canMove = false;
            isReturning = true;
            rb.isKinematic = true;

            for (float f = 0; f < 1f; f += Time.fixedDeltaTime / _returnDuration)
            {
                Vector3 bezieA = Vector3.Lerp(_startPoint, _beziePoint, f);
                Vector3 bezieB = Vector3.Lerp(_beziePoint, _endPoint, f);

                transform.position = Vector3.Lerp(bezieA, bezieB, f);
                yield return new WaitForFixedUpdate();
            }

            canMove = true;
            isReturning = false;
            rb.isKinematic = false;
        }
    }

    private bool GetIsJumping()//Get if it is installed on Ground
    {
        float playerRadius = transform.localScale.x / 2f;
        Vector3 pos = transform.position;
        float length = jump.rayLength + transform.localScale.y;

        var hits = Physics.SphereCastAll(pos, playerRadius, Vector3.down, length, groundLayer, QueryTriggerInteraction.Ignore);

        bool onWater = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "Water")
            {
                onWater = true;
            }
            else if (hit.collider.gameObject.tag == "Ground")
            {
                if (!onWater)
                {
                    if (waitingJump)
                    {
                        waitingJump = false;
                    }
                    hitGroundPoint = hit;
                }
                return false;
            }
        }
        hitGroundPoint = new RaycastHit();
        if (waitingJump)
        {
            waitingJump = false;
        }
        return true;
    }
    private bool IsFlyable()//Check if it is possible to glide
    {
        float playerHeight = transform.localScale.y;
        Vector3 pos = transform.position;
        pos = new Vector3(pos.x, pos.y - playerHeight, pos.z);
        Ray ray = new Ray(pos, -transform.up);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, fly.minDistance))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                Debug.DrawRay(transform.position, -transform.up * fly.minDistance, Color.red, 0.5f);
                return false;
            }
        }
        Debug.DrawRay(transform.position, -transform.up * fly.minDistance, Color.blue, 0.5f);
        return true;

    }
    public (float deceleration, float slopeAngle, Vector3 tangent) SlopeDeceleration(Vector3 velocity)//Set limits on the angles you can go
    {
        float velocityMultiplier = 1.0f;
        Vector3 _tangent = Vector3.zero;
        float _slopeAngle = 0f;
        if (!isJumping && !isFlying)
        {
            Vector3 normal = hitGroundPoint.normal;
            _tangent = new Vector3(normal.x, normal.y - 1, normal.z).normalized;
            if (Mathf.Approximately(normal.y, 0.0f))
            {
                velocityMultiplier = 0.0f;
            }
            else
            {
                Vector2 normalXZ = new Vector2(normal.x, normal.z);
                Vector2 velocityXZ = new Vector2(velocity.x, velocity.z);
                _slopeAngle = Mathf.Rad2Deg * Mathf.Atan2(-Vector2.Dot(normalXZ, velocityXZ) / normal.y, velocityXZ.magnitude);
                float t = Mathf.Clamp01((_slopeAngle - slip.slopeThreshold.Min) / (slip.slopeThreshold.Max - slip.slopeThreshold.Min));
                velocityMultiplier = 1.0f - (t * t * (3.0f - (t * 2.0f)));
            }
            _slopeAngle = Vector3.Angle(normal, Vector3.up);
        }

        return (velocityMultiplier, _slopeAngle, _tangent);
    }

    [Serializable]
    public class Events
    {
        public UnityEvent onMove, onJump, onFly, onFlyEnd, onSlip, onAvoid;
    }
}
