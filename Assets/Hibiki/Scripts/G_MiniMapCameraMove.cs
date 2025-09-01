using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_MiniMapCameraMove : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] RectTransform mapCompass;
    [SerializeField] RectTransform mapPlayer;
    [SerializeField] float cameraHeight = 100f;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, cameraHeight, playerTransform.position.z);
        if (gameManager.setting.otherSetting.isMapRotate)
        {
            float angle = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(90f, angle, 0f);
            mapCompass.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            mapPlayer.rotation = Quaternion.Euler(0f, 0f, -playerTransform.eulerAngles.y);
        }
    }
}
