using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Cinemachine;
using DG.Tweening;

public class P_CameraMove_Alpha : MonoBehaviour
{
    public static P_CameraMove_Alpha instance;
    private CinemachineFreeLook mainFreeLookCamera;
    private Transform cameraTransform;
    private float time;
    private Vector2 cameraSpeed;

    [SerializeField] Transform player;
    [Header("無操作時に戻るまでの時間")]
    [SerializeField] float uncontrolledTime=1f;
    [Header("戻るスピード")]
    [SerializeField] float returnSpeed = 1f;
    public static Vector2 rotateSpeed = new Vector2(1, 1);
    [Header("デフォルトの見下ろし角度(0が最も下、1が最も上)")]
    [SerializeField] [Range(0, 1)] float defaultAngle = 0.5f;
    [SerializeField, Range(0f, 1f)] float battleDefaultAngle = 0.5f;
    private float angleY;
    [Header("プレイヤーとの距離")]
    [SerializeField] [Range(1f,20f)]public float distanceToPlayer = 7f;
    [SerializeField] [Range(1f, 20f)] public float battleDistanceToPlayer = 10f;
    [SerializeField] float switchDistanceDuration = 0.5f;

    [Header("TargetCamera")]
    [SerializeField] Transform targetMark;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Material enemyEmphasisMaterial;
    [SerializeField] Color emphasisColor;
    private Color normalColor;
    [SerializeField] float rotateSqrThreshould = 0.25f;
    [SerializeField] float targetSwitchingMinTime = 0.2f;
    [SerializeField] float targettingSpeed = 5f;
    [SerializeField] float targettingRadius = 10f;
    [SerializeField] float targetPlaceAngle = 0.3f;
    [SerializeField] Vector2 angleYLimit = new Vector2(0.3f, 0.9f);

    [Header("Events")]
    public UnityEvent onAddAngryEnemy,onRemoveAngryEnemy, onAddTargetEnemy, onRemoveTargetEnemy;
    private float targettingTime;

    private List<Transform> angryEnemies;
    private List<Transform> targetEnemies;
    private Transform lastAttackedEnemy;

    private Vector2 _rotate;
    private Vector2 rotateContext;

    private bool isRockOn;

    private GameManager gameManager;


    private void OnValidate()
    {
        mainFreeLookCamera = GetComponent<CinemachineFreeLook>();
        mainFreeLookCamera.m_YAxis.Value = defaultAngle;
        UpdateRotateSpeed(rotateSpeed);
        UpdateDistance(distanceToPlayer);
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        cameraTransform = Camera.main.transform;
        mainFreeLookCamera = GetComponent<CinemachineFreeLook>();
        isRockOn = false;
        time = uncontrolledTime;
        angleY = defaultAngle;
        targettingTime = 0f;
        gameManager.onSettingChanged.AddListener(OnUpdateSetting);
        OnUpdateSetting();
        angryEnemies = new List<Transform>();
        targetEnemies = new List<Transform>();
        lastAttackedEnemy = null;

        normalColor = new Color(emphasisColor.r, emphasisColor.g, emphasisColor.b, 0f);
        //enemyEmphasisMaterial.SetColor("_Color", normalColor);
        SetActiveTargetMark();
    }

    // Update is called once per frame
    void Update()
    {
        //ResetAngleY();

        RockOn();
    }

    private void OnUpdateSetting()
    {
        cameraSpeed = gameManager.setting.oparationSetting.cameraSensitivity;
        UpdateRotateSpeed(cameraSpeed);
    }

    private void RockOn()
    {
        if (!isRockOn) return;

        Vector3 enemyPos;
        if (GetTargetEnemy(out enemyPos))
        {
            targetMark.position = enemyPos + Vector3.up;
            targetMark.LookAt(cameraTransform);
            LookAtTargetXZ(enemyPos);
            LookAtTargetY(enemyPos);
        }
    }

    public void SetLastAttackEnemy(Transform enemy)
    {
        if(targettingTime != 0f)
        {
            return;
        }

        lastAttackedEnemy = enemy;
        StartCoroutine(enumerator());

        IEnumerator enumerator()
        {
            targettingTime = targetSwitchingMinTime;
            while (targettingTime > 0f)
            {
                targettingTime -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            targettingTime = 0f;
        }
    }

    public void DeleteLastAttackEnemy()
    {
        lastAttackedEnemy = null;
    }

    public void AddAngryEnemy(Transform enemy)
    {
        if (!angryEnemies.Contains(enemy))
        {
            angryEnemies.Add(enemy);

            onAddAngryEnemy.Invoke();
        }
        AddTargetEnemy(enemy);
    }
    public void RemoveAngryEnemy(Transform enemy)
    {
        while (angryEnemies.Contains(enemy))
        {
            angryEnemies.Remove(enemy);
        }
        RemoveTargetEnemy(enemy);

        onRemoveAngryEnemy.Invoke();
    }

    public void AddTargetEnemy(Transform enemy)
    {
        if (!targetEnemies.Contains(enemy))
        {
            targetEnemies.Add(enemy);
            onAddTargetEnemy.Invoke();

            if(targetEnemies.Count == 1)
            {
                SwitchCameraAngle(true);
            }
        }
    }
    public void RemoveTargetEnemy(Transform enemy)
    {
        while (targetEnemies.Contains(enemy))
        {
            targetEnemies.Remove(enemy);
        }

        onRemoveTargetEnemy.Invoke();
        if (targetEnemies.Count == 0)
        {
            isRockOn = false;
            SetActiveTargetMark();
            SwitchCameraAngle(false);
        }
    }

    public bool ExistAngryEnemy()
    {
        return (angryEnemies.Count > 0);
    }

    public void SetRockOn(bool value)
    {
        isRockOn = value;
        SetActiveTargetMark();
    }
    public void SwitchRockOn()
    {
        isRockOn = !isRockOn;
        SetActiveTargetMark();
    }

    private void SetTargetting()
    {
        Vector3 playerPosition = player.position;
        Vector3 cameraDirection = cameraTransform.forward;
        Vector3 pos = playerPosition + cameraDirection * targettingRadius;
        var target = Physics.OverlapSphere(pos, targettingRadius, enemyLayer).Select(enemy => enemy.transform).OrderByDescending(enemy => Vector3.Dot(cameraDirection, (enemy.position - playerPosition))).FirstOrDefault();
        if (target == null) return;

        AddTargetEnemy(target);
        isRockOn = true;
        SetActiveTargetMark();
    }

    private void SetActiveTargetMark()
    {
        targetMark.gameObject.SetActive(isRockOn);
    }
    

    private void ResetAngleY()
    {
        if (time == uncontrolledTime)
        {
            mainFreeLookCamera.m_YAxis.Value = Mathf.Lerp(mainFreeLookCamera.m_YAxis.Value, defaultAngle, Time.deltaTime * returnSpeed);
        }
        if (mainFreeLookCamera.m_YAxis.m_InputAxisValue + mainFreeLookCamera.m_XAxis.m_InputAxisValue == 0)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        if (time > uncontrolledTime) time = uncontrolledTime;

        if (_rotate != rotateSpeed)
        {
            _rotate = rotateSpeed;
            UpdateRotateSpeed(rotateSpeed);
        }
    }

    public void SwitchCameraAngle(bool isBattle)
    {
        angleY = isBattle ? battleDefaultAngle : defaultAngle;
        float _distance = isBattle ? battleDistanceToPlayer : distanceToPlayer;
        float startDistance = mainFreeLookCamera.m_Orbits[0].m_Height;

        DOVirtual.Float(startDistance, _distance, switchDistanceDuration, d =>
           {
               UpdateDistance(d);
           }
        ).SetEase(Ease.OutCubic);
    }

    private bool GetTargetEnemy(out Vector3 position)
    {
        if(targetEnemies.Count == 0)
        {
            position = Vector3.zero;
            return false;
        }

        if(rotateContext.sqrMagnitude > rotateSqrThreshould)
        {
            position = Vector3.zero;
            if(lastAttackedEnemy != null)
            {
                DeleteLastAttackEnemy();
            }

            return false;
        }

        if (lastAttackedEnemy != null)
        {
            if (targetEnemies.Contains(lastAttackedEnemy))
            {
                position = lastAttackedEnemy.position;
                return true;
            }
        }

        Vector3 playerPosition = player.position;
        Vector3 cameraDirection = cameraTransform.forward;


        {
            position = targetEnemies.Select(enemy => enemy.position).OrderByDescending(enemy => Vector3.Dot(cameraDirection, (enemy - playerPosition))).FirstOrDefault();
            return true;
        }
    }

    private void LookAtTargetXZ(Vector3 target)
    {
        // それぞれの座標をカメラの高さに合わせる.
        float cameraHeight = mainFreeLookCamera.transform.position.y;
        Vector3 followPosition =
            new Vector3(mainFreeLookCamera.Follow.position.x, cameraHeight, mainFreeLookCamera.Follow.position.z);
        Vector3 targetPosition =
            new Vector3(target.x, cameraHeight, target.z);

        // それぞれのベクトルを計算.
        Vector3 followToTarget = targetPosition - followPosition;
        Vector3 followToTargetReverse = Vector3.Scale(followToTarget, new Vector3(-1, 1, -1));
        Vector3 followToCamera = mainFreeLookCamera.transform.position - followPosition;

        // カメラ回転の角度と方向を計算.
        Vector3 axis = Vector3.Cross(followToCamera, followToTargetReverse);
        float direction = axis.y < 0 ? -1 : 1;
        float angle = Vector3.Angle(followToCamera, followToTargetReverse);

        mainFreeLookCamera.m_XAxis.Value = angle * direction * targettingSpeed * Time.deltaTime;
    }
    private void LookAtTargetY(Vector3 targetPosition)
    {
        Vector3 playerPosition = player.position + Vector3.down;

        float angle = Mathf.Clamp(((playerPosition - targetPosition).normalized.y * 0.5f) + 0.5f + targetPlaceAngle,angleYLimit.x,angleYLimit.y);


        mainFreeLookCamera.m_YAxis.Value = Mathf.Lerp(mainFreeLookCamera.m_YAxis.Value, angle, Time.deltaTime * targettingSpeed);
    }

    public void UpdateRotateSpeed(Vector2 newSpeed)
    {
        mainFreeLookCamera.m_XAxis.m_MaxSpeed = 180 * newSpeed.x;
        mainFreeLookCamera.m_YAxis.m_MaxSpeed = newSpeed.y;
    }

    public void LockCameraRotate(bool toLock)
    {
        if (toLock)
        {
            cameraSpeed = GetRotateSpeed();
            UpdateRotateSpeed(Vector2.zero);
        }
        else
        {
            UpdateRotateSpeed(cameraSpeed);
        }
    }
    public Vector2 GetRotateSpeed()
    {
        return new Vector2(mainFreeLookCamera.m_XAxis.m_MaxSpeed/180f, mainFreeLookCamera.m_YAxis.m_MaxSpeed);
    }
    public void UpdateDistance(float distance)
    {
        mainFreeLookCamera.m_Orbits[0].m_Height = distance;
        mainFreeLookCamera.m_Orbits[1].m_Radius = distance;
        mainFreeLookCamera.m_Orbits[2].m_Height = -distance;
    }

    public void EmphasisEnemy(bool activeSelf)
    {
        Color _color = enemyEmphasisMaterial.GetColor("_Color");
        
        DOVirtual.Color(_color, (activeSelf ? emphasisColor : normalColor), 0.5f, c =>
        {
            enemyEmphasisMaterial.SetColor("_Color", c);
        }).Kill(true);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateContext = context.ReadValue<Vector2>() * rotateSpeed;
    }

    public void OnRockOn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //EmphasisEnemy(true);
            if(targetEnemies.Count == 0)
            {
                SetTargetting();
            }
            else if(angryEnemies.Count == 0)
            {
                targetEnemies.Clear();
                isRockOn = false;
                SetActiveTargetMark();
            }
            else
            {
                SwitchRockOn();
            }

           
            if (isRockOn)
            {
                S_SEManager.PlayTargetCameraOnSE(transform);
            }
            else
            {
                S_SEManager.PlayTargetCameraOffSE(transform);
            }
        }
        //else if (context.canceled)
        //{
        //    EmphasisEnemy(false);
        //}
        
    }
}
