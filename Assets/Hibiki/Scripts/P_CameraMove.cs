using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class P_CameraMove : MonoBehaviour
{
    public CinemachineVirtualCamera mainCameraCinemachine;
    private CinemachineTransposer mainCameraOffset;
    public P_PlayerMove move;

    [Header("調整可能な値")]
    [Header("右スティック未入力時の待機時間")]
    [SerializeField] float nonLookInputMaxCount = 5f;
    [Header("進行方向を向くスポード")]
    [SerializeField] float mainCameraFlollowSpeed = 1;
    [Header("右スティック感度")]
    [SerializeField] Vector2 cameraRotateSensitivilty = new Vector2(1, 1);
    [Header("カメラとの距離")]
    [SerializeField] float distanceToMainCamera = 9;
    [Header("デフォルトの見下ろし角度")]
    [SerializeField] float defaultAngleX = 20;
    private float cameraAngleX;


    private Vector3 defaultMainCameraPosition;
    private Vector3 cameraVec;
    private Vector2 cameraAxis;


    private float nonLookInputCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        cameraAngleX = defaultAngleX;
        mainCameraOffset = mainCameraCinemachine.GetCinemachineComponent<CinemachineTransposer>();
        defaultMainCameraPosition = mainCameraOffset.m_FollowOffset;

    }

    // Update is called once per frame
    void Update()
    {
        //MainCamera adjusting the angle of view
        //CameraRotate();

    }


    //private void CameraRotate()
    //{
    //    if (cameraAxis.sqrMagnitude != 0) nonLookInputCount = nonLookInputMaxCount;
    //    cameraVec = new Vector3(0, 100*cameraRotateSensitivilty.x * cameraAxis.x, 0);
    //    cameraAngleX -= cameraRotateSensitivilty.y * cameraAxis.y;
    //    if (cameraAngleX > 89)
    //    {
    //        cameraAngleX = 89;
    //    }
    //    else if (cameraAngleX < -89)
    //    {
    //        cameraAngleX = -89;
    //    }

    //    if (nonLookInputCount > 0)//OnInput
    //    {
    //        nonLookInputCount -= Time.deltaTime;
    //        move.cameraTransform.eulerAngles += cameraVec * Time.deltaTime;
    //        mainCameraOffset.m_FollowOffset = distanceToMainCamera * move.PositioningFromAngle(cameraAngleX);

    //    }
    //    else if (nonLookInputCount < 0) nonLookInputCount = 0;//NonInput
    //    else if(move.moveAxis.sqrMagnitude != 0)
    //    {
    //        //MainCamera follow
    //        if (Mathf.Abs(move.moveAxis.x) >= 0.1f)
    //        {
    //            move.cameraTransform.rotation = Quaternion.Lerp(move.cameraTransform.rotation, transform.rotation, Time.deltaTime * mainCameraFlollowSpeed);
    //        }
    //        cameraAngleX = defaultAngleX;
    //        mainCameraOffset.m_FollowOffset = Vector3.Lerp(mainCameraOffset.m_FollowOffset, defaultMainCameraPosition, Time.deltaTime);
    //    }
    //}


    public void OnLook(InputAction.CallbackContext context)//Get controller camera rotation input
    {
        cameraAxis = context.ReadValue<Vector2>();
    }
}
