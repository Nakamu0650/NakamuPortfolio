using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class P_SunFlowerAttack : P_WreathAttack_Base
{
    [Header("Absolute")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject strageEffect;
    [SerializeField] GameObject shotEffectGameObject;
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject mainCamera;
    [SerializeField] Transform bulletLine;
    [SerializeField] LayerMask flowerLayer;
    [SerializeField] Transform summonPoint;
    [SerializeField] Vector3 holdingPoint;
    [SerializeField] Vector3 shootingPoint;

    [Header("Varidate")]
    [SerializeField] int useEnergyAmount = 1;
    [SerializeField] float useEnergySpan = 0.5f;

    [SerializeField] float minStorageTime = 0.3f;
    [SerializeField] float maxStorageTime = 2f;

    [SerializeField] float minBeamLength = 10f;
    [SerializeField] float maxBeamLength = 40f;

    [SerializeField] float minBeamRadius = 0.2f;
    [SerializeField] float maxBeamRadius = 1f;

    [SerializeField] float minActivateDuration = 0.4f;
    [SerializeField] float maxActivateDuration = 1f;

    [SerializeField] float attackSpan = 0.2f;

    [SerializeField] float minCameraImpuse = 1f;
    [SerializeField] float maxCameraImpuse = 4f;

    [SerializeField] float minControllerImpuse = 0.1f;
    [SerializeField] float maxControllerImpuse = 0.4f;

    [SerializeField] float releaseDuration = 1f;

    [Header("Animation")]
    [SerializeField] string chargeAnimation;
    [SerializeField] string attackAnimation;

    private LayerMask hitLayer;
    private bool isReleased = false;
    private bool isHolding = false;
    private float ct = 0f;
    private Vector2 axis;
    private G_ShakeCamera shakeCamera;
    private G_ShakeController shakeController;
    private CinemachineTransposer cinemachineTransposer;

    private void OnValidate()
    {
        maxBeamLength = Mathf.Max(minBeamLength, maxBeamLength);
        minBeamLength = Mathf.Max(minBeamLength, 0f);

        maxBeamRadius = Mathf.Max(minBeamRadius, maxBeamRadius);
        minBeamRadius = Mathf.Max(minBeamRadius, 0f);

        maxActivateDuration = Mathf.Max(minActivateDuration, maxActivateDuration);
        minActivateDuration = Mathf.Max(minActivateDuration, 0f);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        isReleased = false;
        isHolding = false;
        ct = 0f;
        bulletLine.gameObject.SetActive(false);
        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        shakeCamera = G_ShakeCamera.instance;
        shakeController = G_ShakeController.instance;
        hitLayer = enemyLayer + flowerLayer;
    }

    public override void OnPressButton()
    {
        base.OnPressButton();

        StartCoroutine(shot());

        G_BloomProvidenceAnalysisSystem.instance.data.sunflower++;
        IEnumerator shot()
        {
            var nearestEnemy = GetNearestEnemy();
            if (nearestEnemy)
            {
                Vector3 _forward = nearestEnemy.transform.position - transform.position;
                _forward = new Vector3(_forward.x, 0f, _forward.z);
                Quaternion lookAt = Quaternion.LookRotation(_forward);
                for (float t = 0; t < 1f; t += Time.deltaTime / minStorageTime)
                {
                    player.rotation = Quaternion.Lerp(transform.rotation, lookAt, t);
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(minStorageTime);
            }

            if (!UseEnergy(useEnergyAmount))
            {
                yield break;
            }

            StartCoroutine(Shot(0f));
        }

    }

    public override void OnHoldButton()
    {
        if (!UseEnergy(useEnergyAmount))
        {
            return;
        }
        base.OnHoldButton();

        G_BloomProvidenceAnalysisSystem.instance.data.sunflower++;
        GameObject strage = Instantiate(strageEffect, transform);
        S_SEManager.PlaySunflowerLaserChargeSE(transform);
        strage.transform.position = summonPoint.position;
        strage.transform.rotation = summonPoint.rotation;
        float playerAngleY = mainCamera.transform.eulerAngles.y;
        player.rotation = Quaternion.Euler(0f, playerAngleY, 0f);
        Quaternion lookAt = Quaternion.Euler(0f, playerAngleY, 0f);
        axis = Vector2.zero;
        playerMove.modelAnimator.SetBool(chargeAnimation, true);
        StartCoroutine(holding());

        IEnumerator holding()
        {
            float _holdingTime = 0f;
            cinemachineTransposer.m_FollowOffset = holdingPoint;
            isHolding = true;
            cameraObject.SetActive(true);
            playerMove.canMove = false;
            playerMove.isKinematic = true;
            playerMove.rb.velocity = Vector3.zero;

            for (float t = 0; t < 1f; t += Time.deltaTime / minStorageTime)
            {
                //player.rotation = Quaternion.Lerp(transform.rotation, lookAt, t);
                yield return null;
            }

            UpdateBulletLineSize(minBeamRadius, minBeamLength);
            bulletLine.gameObject.SetActive(true);

            float length01 = 0f;

            float useTime = 0f;

            while (!isReleased)
            {
                _holdingTime += Time.deltaTime;
                useTime += Time.deltaTime;
                summonPoint.localEulerAngles += new Vector3(-axis.y, 0f, 0f);
                player.eulerAngles += new Vector3(0f, axis.x, 0f);
                if (_holdingTime >= maxStorageTime)
                {
                    _holdingTime = maxStorageTime;
                    break;
                }
                if(useTime >= useEnergySpan)
                {
                    useTime = 0f;
                    if (!UseEnergy(useEnergyAmount))
                    {
                        break;
                    }
                }
                length01 = Mathf.InverseLerp(0f, maxStorageTime, _holdingTime);
                UpdateBulletLineSize(Mathf.Lerp(minBeamRadius, maxBeamRadius, length01), Mathf.Lerp(minBeamLength, maxBeamLength, length01));
                yield return null;
            }
            bulletLine.gameObject.SetActive(false);

            StartCoroutine(Shot(length01));
            Destroy(strage);

            for (float f = 0f; f < 1f; f += Time.deltaTime / releaseDuration)
            {
                cinemachineTransposer.m_FollowOffset = Vector3.Lerp(holdingPoint, shootingPoint, Mathf.Sin(0.5f * Mathf.PI * f));
                yield return null;
            }

            cameraObject.SetActive(false);
            isReleased = false;
            isHolding = false;
            playerMove.canMove = true;
            playerMove.rb.velocity = Vector3.zero;

        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (isHolding)
        {
            isReleased = true;
        }
    }

    private IEnumerator Shot(float _beam01)
    {
        playerMove.canMove = false;
        playerMove.isKinematic = true;
        S_SEManager.SEStop("sunflowerLaserCharge");
        float _beamLength = Mathf.Lerp(minBeamLength, maxBeamLength, _beam01);
        float _beamRadius = Mathf.Lerp(minBeamRadius, maxBeamRadius, _beam01);
        float _activateDuration = Mathf.Lerp(minActivateDuration, maxActivateDuration, _beam01);
        if (_beam01 >= 0.95f)
        {
            S_SEManager.PlaySunflowerLaserShot3SE(transform);
        }
        else if(_beam01 > 0.5)
        {
            S_SEManager.PlaySunflowerLaserShot2SE(transform);
        }
        else
        {
            S_SEManager.PlaySunflowerLaserShot1SE(transform);
        }

        playerMove.modelAnimator.SetTrigger(attackAnimation);
        playerMove.modelAnimator.SetBool(chargeAnimation, false);

        shakeCamera.ShakeCamera(Mathf.Lerp(minCameraImpuse, maxCameraImpuse, _beam01));

        float controllerShakeAmount = Mathf.Lerp(minControllerImpuse, maxControllerImpuse, _beam01);
        shakeController.ShakeController(controllerShakeAmount, controllerShakeAmount, _activateDuration);

        Vector3 startPos = summonPoint.position;

        Vector3 shotFoward = summonPoint.forward * _beamLength;

        Transform effect = Instantiate(shotEffectGameObject, startPos, summonPoint.rotation).transform;

        SetShotParticle(effect, minActivateDuration, _beamLength, _beamRadius, _beamLength / minActivateDuration, (_beam01 == 1f));

        for (float t = 0f; t < _activateDuration; t += attackSpan)
        {
            Vector3 endPos = startPos + (shotFoward * Mathf.InverseLerp(0f, minActivateDuration, t));
            var cols = Physics.OverlapCapsule(startPos, endPos, _beamRadius, hitLayer);
            foreach (var col in cols)
            {
                if (col.gameObject.CompareTag("Flower"))
                {
                    col.GetComponent<G_FlowerGroup>().Bloom();
                }
                else if (col.gameObject.CompareTag("Enemy"))
                {
                    col.GetComponent<F_HP>().OnDamage(playerParam.GetATK(), attackValue.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(transform.position, col.transform.position));
                    S_SEManager.PlaySunflowerLaserHitSE(transform);
                }
            }
            yield return new WaitForSeconds(attackSpan);
        }
        summonPoint.transform.rotation = transform.rotation;
        playerMove.canMove = true;
        playerMove.isKinematic = false;
    }

    private void UpdateBulletLineSize(float radius, float length)
    {
        bulletLine.transform.localScale = new Vector3(radius, radius, length);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!isHolding) return;

        axis = Vector2.Scale(context.ReadValue<Vector2>(), gameManager.setting.oparationSetting.fpsCameraSensitivity);
    }

    private void SetShotParticle(Transform particleObject, float attackTime, float attackLength, float radius, float speed, bool exceedTheSound)
    {
        ParticleSystem beam = particleObject.GetChild(0).GetComponent<ParticleSystem>();
        ParticleSystem beam1 = particleObject.GetChild(1).GetComponent<ParticleSystem>();
        GameObject flower = particleObject.GetChild(2).gameObject;
        Transform shockWave = particleObject.GetChild(3);
        Transform shockWave2 = particleObject.GetChild(4);

        if (!exceedTheSound)
        {
            Destroy(flower.gameObject);
            Destroy(shockWave.gameObject);
            Destroy(shockWave2.gameObject);
        }
        else
        {
            Vector3 forword = particleObject.forward * attackLength / 3f;

            shockWave.position = particleObject.position + forword;
            shockWave2.position = particleObject.position + (forword * 2f);
        }

        SetStartLifeTime(beam, attackTime);
        SetStartLifeTime(beam1, attackTime);

        SetStartSpeed(beam, speed);
        SetStartSpeed(beam1, speed);

        SetStartSize(beam, 2 * radius);
    }

    private void SetStartLifeTime(ParticleSystem effect, float value)
    {
        var _mainModule = effect.main;
        var _minMaxCurve = _mainModule.startLifetime;
        var _animationCurve = _mainModule.startLifetime.curve;

        _minMaxCurve = value;
        _mainModule.startLifetime = _minMaxCurve;
    }

    private void SetStartSize(ParticleSystem effect, float value)
    {
        var _mainModule = effect.main;
        var _minMaxCurve = _mainModule.startSize;
        var _animationCurve = _mainModule.startSize.curve;

        _minMaxCurve = value;
        _mainModule.startSize = _minMaxCurve;
    }

    private void SetStartSpeed(ParticleSystem effect, float value)
    {
        var _mainModule = effect.main;
        var _minMaxCurve = _mainModule.startSpeed;
        var _animationCurve = _mainModule.startLifetime.curve;

        _minMaxCurve = value;
        _mainModule.startSpeed = _minMaxCurve;
    }
}
