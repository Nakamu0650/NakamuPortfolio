using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class riverTest : MonoBehaviour
{
    [SerializeField] CriWare.Assets.CriAtomCueReference splineSound;
    [SerializeField] GameObject TargetObj;
    [SerializeField] Vector3 RotateAxis = Vector3.up;
    [SerializeField] float Speed = 0.1f;
    [SerializeField] Transform listenerTransform;
    [SerializeField] Transform CameraTransrom;
    private S_SoundManager manager;
    [SerializeField] string audioKey;
    // Start is called before the first frame update
    void Start()
    {
        manager = S_SoundManager.Instance;
        manager.PlaySound(audioKey, splineSound.AcbAsset.Handle, splineSound.CueId, transform, true, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (TargetObj == null)
            {
                return;
            }
            else
            {
                transform.RotateAround(TargetObj.transform.position, RotateAxis, 360 / (1.0f / Speed) * Time.deltaTime);

            }
        }
        manager.UpdateListenerPosition(CameraTransrom);

        manager.UpdateSoundPosition(audioKey);
    }
}
