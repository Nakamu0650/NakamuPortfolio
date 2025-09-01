using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundtest : MonoBehaviour
{
    S_SoundManager S_Sound;
    [SerializeField] Transform Lisener;
    [SerializeField] Transform CameraTransrom;
    [SerializeField] CriWare.Assets.CriAtomCueReference cueReference1;
    [SerializeField] CriWare.Assets.CriAtomCueReference cueReference2;
    [SerializeField] CriWare.Assets.CriAtomCueReference cueReference3;
    [Header("3DPositioningTest")][Tooltip("Press P")][SerializeField] CriWare.Assets.CriAtomCueReference PositioningTest;
    void Start()
    {
        S_Sound = S_SoundManager.Instance;
        S_Sound.SetListenerTransform(Lisener.transform);
    }
    private void Update()
    {
        S_Sound.UpdateListenerPosition(CameraTransrom);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);
            
            S_Sound.PlaySound("example", cueReference1.AcbAsset.Handle, cueReference1.CueId, transform, false);
            Debug.Log("cue1:"+cueReference1.CueId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);

            S_Sound.PlaySound("example1", cueReference2.AcbAsset.Handle, cueReference2.CueId, transform, false);
            Debug.Log("cue2:" + cueReference2.CueId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);

            S_Sound.PlaySound("example2", cueReference3.AcbAsset.Handle, cueReference3.CueId, transform, false);
            Debug.Log("cue3:" + cueReference3.CueId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);
            
            //S_SEManager.PlayboarVoiceSE(transform);
            
            Debug.Log("cue1:" + cueReference1.CueId);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);
            
            S_Sound.PlaySound("test", PositioningTest.AcbAsset.Handle, PositioningTest.CueId, transform, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);

            S_Sound.Stop("example");
            Debug.Log("cue1:" + cueReference1.CueId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);
            S_Sound.Stop("example1");
            
            Debug.Log("cue2:" + cueReference2.CueId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            //S_Sound.PlaySound("attack", attackSound,0, transform, false);

            S_Sound.Stop("example2");
            Debug.Log("cue3:" + cueReference3.CueId);
        }
    }
}
