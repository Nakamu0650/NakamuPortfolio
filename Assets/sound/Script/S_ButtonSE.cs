using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ButtonSE : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySelectedSE()
    {
        //Debug.Log("select");
        S_SEManager.PlaySelectSE(transform);
    }

    public void PlayStartSelectSE()
    {
        S_SEManager.PlayStartSelectSE(transform);
    }
}
