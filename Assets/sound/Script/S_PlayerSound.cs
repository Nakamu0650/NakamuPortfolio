using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerSound : MonoBehaviour
{
    bool isRotate = false;
    public static bool isMove = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayAttackSound()
    {
        S_SEManager.PlayPlayerAttackSE(transform);
    }

    void PlayRoseSwordSE()
    {
        S_SEManager.PlayRoseAttackSE(transform);
    }

    void PlayRoseRotationSwordSE()
    {
        if (!isRotate)
        {
            S_SEManager.PlayRoseRotationAttackSE(transform);
            isRotate = true;
        }
       
    }

    void StopRotateSound()
    {
        if(isRotate)
        {
            S_SEManager.PlayRoseRotationAttackStopSE(transform);
            isRotate = false;
        }
    }

    void StopFlySE()
    {
        S_SEManager.SEStop("playerfly");
    }

    public void OnMove()
    {
        //isMove = true;
        S_SEManager.PlayPlayerWalkSE(transform);
        if (G_SeasonManager.season == G_SeasonManager.Season.Winter)
        {
            S_SEManager.SEControl("playerWalk", 0, 0.1f, S_SEManager.playerWalkAisacName[0], 0);
        }
        else
        {
            S_SEManager.SEControl("playerWalk", 1, 0.1f, S_SEManager.playerWalkAisacName[0], 0);
        }
    }



    public void Brake()
    {
        if(isMove)
        {
            //S_SEManager.PlayPlayerBrakeSE(transform);
            isMove= false;
            S_SEManager.SEStop("playerWalk");
        }

    }
}
