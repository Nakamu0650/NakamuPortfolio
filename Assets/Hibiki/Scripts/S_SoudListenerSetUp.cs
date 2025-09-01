using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SoudListenerSetUp : MonoBehaviour
{
    private S_SoundManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = S_SoundManager.Instance;
        manager.SetListenerTransform(transform);
    }
}
