using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBoarSound : MonoBehaviour
{
    S_SoundManager S_Sound;
    // Start is called before the first frame update
    void Start()
    {
        //S_SEManager.PlayboarVoice2SE(transform);
    }

    // Update is called once per frame
    void Update()
    {
        S_Sound.UpdateSoundPosition("boarvoice2");
        Debug.Log("boar" + transform);
    }
}
