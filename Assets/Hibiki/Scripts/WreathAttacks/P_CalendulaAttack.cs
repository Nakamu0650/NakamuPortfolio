using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using EggSystem;

public class P_CalendulaAttack : P_WreathAttack_Base
{
    [Header("Absolute")]
    [SerializeField] GameObject bombObjectPrefab;
    [SerializeField] GameObject bombExplosionEffectPrefab;
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject mainCamera;

    [SerializeField] LineRenderer[] bombTraces;

    [SerializeField] Transform summonPoint;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask flowerLayer;

    [SerializeField] float gravityScale;

    [Header("Variable")]
    [SerializeField] float bombRadius = 0.5f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float onceAngle = 20f;
    [SerializeField] float throwSpan = 0.05f;
    [SerializeField] float bombLifeTime = 10f;

    [SerializeField] int addBombAmount = 1;

    [SerializeField] int useEnergyAmount = 3;

    [SerializeField] RangeInt bombAmountRange;
    [SerializeField] Range<float> holdingDuration;
    [SerializeField] Range<float> pushPowerRange;
    [SerializeField] Range<float> pushPowerAttenuationRange;
    [SerializeField] Range<float> cameraShakePower;
    [SerializeField] Range<float> controllerShakePower;
    [SerializeField] Range<float> controllerShakeDuration;
    [SerializeField] Range<float> shakeDistance;
    [SerializeField] AnimationCurve pushPowerCurve;

    [Header("Animations")]
    [SerializeField] string onChargeAnimation, onAttackAnimation;

    private LayerMask bombHitLayer;
    private LayerMask explosionHitLayer;
    private Vector2 axis;
    private Range<float> cameraShakeSqrDistance;
    private bool isHolding;
    private float lineAmountPercentage;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        bombHitLayer = groundLayer + enemyLayer;
        explosionHitLayer = enemyLayer + flowerLayer;
        lineAmountPercentage = addBombAmount * 1f / (float)(bombTraces.Length -1);
        cameraShakeSqrDistance = shakeDistance.Select(value => Mathf.Pow(value, 2f));

        SetActiveBombTraces(0);
    }

    public override void OnPressButton()
    {
        if (!UseEnergy(useEnergyAmount))
        {
            return;
        }

        base.OnPressButton();
        isHolding = false;

        Vector3 direction = (summonPoint.forward + Vector3.up).normalized;

        G_BloomProvidenceAnalysisSystem.instance.data.calendura++;

        playerMove.modelAnimator.SetTrigger(onAttackAnimation);
        playerMove.modelAnimator.SetBool(onChargeAnimation, false);

        Debug.Log("OnPressButton");
        StartCoroutine(Throw(pushPowerRange.Min, direction));
    }

    public override void OnHoldButton()
    {
        if (!UseEnergy(useEnergyAmount))
        {
            return;
        }
        base.OnHoldButton();
        cameraObject.SetActive(true);
        isHolding = true;
        axis = Vector2.zero;
        float playerAngleY = mainCamera.transform.eulerAngles.y;
        player.rotation = Quaternion.Euler(0f, playerAngleY, 0f);
        Quaternion lookAt = Quaternion.Euler(0f, playerAngleY, 0f);
        StartCoroutine(holding());

        G_BloomProvidenceAnalysisSystem.instance.data.calendura++;

        IEnumerator holding()
        {
            float holdingTime = holdingDuration.Min;
            S_SEManager.PlayBalsamDistanceSE(transform);
            playerMove.canMove = false;
            isHolding = true;
            playerMove.rb.velocity = Vector3.zero;
            playerMove.isKinematic = true;

            playerMove.modelAnimator.SetBool(onChargeAnimation, true);

            for (float t = 0; t < 1f; t += Time.deltaTime / holdingDuration.Min)
            {
                player.rotation = Quaternion.Lerp(transform.rotation, lookAt, t);
                yield return null;
            }

            bool canAdd = true;
            float holding01 = 0f;
            int amount = 1;
            int nextAmount = 1;
            Vector3[] directions = new Vector3[bombTraces.Length];
            float[] pushPowers = new float[bombTraces.Length];
            while (isHolding)
            {
                summonPoint.localEulerAngles += new Vector3(-axis.y, 0f, 0f);
                player.eulerAngles += new Vector3(0f, axis.x, 0f);
                holding01 = Mathf.InverseLerp(holdingDuration.Min, holdingDuration.Max, holdingTime);

                amount = 1 + Mathf.FloorToInt(holding01 / lineAmountPercentage);
                
                if(canAdd &&(amount == nextAmount))
                {
                    S_SEManager.PlayBalsamChargeSE(transform);
                    nextAmount += 1;
                    if (!UseEnergy(useEnergyAmount))
                    {
                        canAdd = false;
                    }
                }
                SetActiveBombTraces(amount);

                float defaultYAngle = -(float)amount * onceAngle / 2f;
                for (int i = 0; i < amount; i++)
                {
                    Quaternion rotation = Quaternion.Euler(0f, defaultYAngle + (i * onceAngle), 0f);
                    directions[i] = rotation * summonPoint.forward;
                    float attenuation = Mathf.Abs(Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f, (float)amount, (float)i)));
                    pushPowers[i] = 1 - Mathf.Lerp(pushPowerAttenuationRange.Min, pushPowerAttenuationRange.Max, attenuation);
                    ShowLineRenderer(bombTraces[i], Mathf.Lerp(pushPowerRange.Min, pushPowerRange.Max, Mathf.Clamp01(pushPowerCurve.Evaluate(holding01))) * pushPowers[i], directions[i]);
                }
                holdingTime += Time.deltaTime;
                yield return null;
            }

            SetActiveBombTraces(0);

            playerMove.modelAnimator.SetTrigger(onAttackAnimation);
            playerMove.modelAnimator.SetBool(onChargeAnimation, false);

            Debug.Log($"amount:{amount}");
            for (int i = 0; i < amount; i++)
            {
                StartCoroutine(Throw(Mathf.Lerp(pushPowerRange.Min, pushPowerRange.Max, holding01) * pushPowers[i], directions[i]));
                yield return new WaitForSeconds(throwSpan);
            }
            isHolding = false;
            playerMove.canMove = true;
            playerMove.rb.velocity = Vector3.zero;
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        cameraObject.SetActive(false);
        isHolding = false;
    }


    private void ShowLineRenderer(LineRenderer lineRenderer, float pushPower, Vector3 pushDirection)
    {
        Vector3 direction = Time.fixedDeltaTime * pushDirection;
        Vector3 position = summonPoint.position;

        lineRenderer.positionCount = 0;

        bool isGround = false;
        int count = 0;
        while (!isGround && count++<128)
        {
            lineRenderer.positionCount = count;
            lineRenderer.SetPosition(count -1, position);

            position += pushPower * direction;
            direction += gravityScale * Vector3.up * Time.fixedDeltaTime;

            var cols = Physics.OverlapSphere(position, bombRadius, bombHitLayer);
            cols = cols.Where(col => (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Ground"))).ToArray();
            isGround = (cols.Length != 0);
        }
    }

    private IEnumerator Throw(float pushPower, Vector3 pushDirection)
    {
        pushPower = Mathf.Max(pushPower, 1f);
        if(pushDirection == Vector3.zero)
        {
            pushDirection = (summonPoint.forward + Vector3.up).normalized;
        }

        GameObject bombObject = Instantiate(bombObjectPrefab, summonPoint.position, Quaternion.identity);
        Destroy(bombObject, bombLifeTime);
        Transform bombTransform = bombObject.transform;
        S_SEManager.SEStop("balsamDistance");
        Vector3 direction = Time.fixedDeltaTime * pushDirection;
        playerMove.isKinematic = false;

        bool isGround = false;
        Vector3 beforePosition = bombTransform.position;
        while (!isGround)
        {
            bombTransform.position += pushPower * direction;
            direction += gravityScale * Vector3.up * Time.fixedDeltaTime;

            var cols = Physics.OverlapCapsule(beforePosition, bombTransform.position, bombRadius, bombHitLayer);
            cols = cols.Where(col => (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Ground"))).ToArray();
            isGround = (cols.Length != 0);

            yield return new WaitForFixedUpdate();
        }
        Explosion(bombTransform.position);
        Destroy(bombObject);
    }

    private void Explosion(Vector3 position)
    {
        bool isFlowerHit = false;
        bool isEnemyHit = false;
        S_SEManager.PlayBalsamHitLandingSE(transform);
        Instantiate(bombExplosionEffectPrefab, position, Quaternion.Euler(-90f, Random.Range(0f ,360f), 0f)).transform.localScale = explosionRadius * Vector3.one;
        float clamp = 1f - Mathf.InverseLerp(cameraShakeSqrDistance.Min, cameraShakeSqrDistance.Max, (Vector3.SqrMagnitude(position - transform.position)));
        float cameraPower = Mathf.Lerp(cameraShakePower.Min, cameraShakePower.Max, clamp);
        float controllerPower = Mathf.Lerp(controllerShakePower.Min, controllerShakePower.Max, clamp);
        float controllerDuration = Mathf.Lerp(controllerShakeDuration.Min, controllerShakeDuration.Max, clamp);
        G_ShakeCamera.instance.ShakeCamera(cameraPower);
        G_ShakeController.instance.ShakeController(controllerPower, controllerPower, controllerDuration);
        var cols = Physics.OverlapSphere(position, explosionRadius, explosionHitLayer);
        foreach (var col in cols)
        {
            if (col.gameObject.CompareTag("Flower"))
            {
                col.gameObject.GetComponent<G_FlowerGroup>().Bloom();
                if (!isFlowerHit)
                {
                    
                    isFlowerHit = true;
                }
                
            }
            else if (col.gameObject.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<F_HP>().OnDamage(playerParam.GetATK(), attackValue.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(position, col.transform.position));
                if (!isEnemyHit)
                {
                    S_SEManager.PlayBalsamHitSE(transform);
                    isEnemyHit = true;
                }
            }
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!isHolding) return;

        axis = Vector2.Scale(context.ReadValue<Vector2>(), gameManager.setting.oparationSetting.fpsCameraSensitivity);
    }

    /// <summary>
    /// Set bombTraces's activeSelves.
    /// If you want hidden all, set amount 0.
    /// </summary>
    /// <param name="amount"></param>
    private void SetActiveBombTraces(int amount)
    {
        for(int i = 0; i < bombTraces.Length; i++)
        {
            bombTraces[i].gameObject.SetActive(i < amount);
        }
    }
}
