using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_PlayerAnimation : MonoBehaviour
{
    [SerializeField] S_SoundManager soundManager;
    [SerializeField] CriWare.Assets.CriAtomCueReference walkSound,landingSound;
    [SerializeField] GameObject umbrellaObj;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = S_SoundManager.Instance;
    }
    public void PlayWalkSound()
    {
        soundManager.PlaySound("walk", walkSound.AcbAsset.Handle, walkSound.CueId,transform, false);
    }
    public void PlayLandingSound()
    {
        soundManager.PlaySound("landing", landingSound.AcbAsset.Handle, landingSound.CueId, transform, false);
    }
}
