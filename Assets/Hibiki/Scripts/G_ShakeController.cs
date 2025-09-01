using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class G_ShakeController : MonoBehaviour
{
    public static G_ShakeController instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShakeController(float lowFrequency,float highFrequency,float duration)
    {
        var gamepad = Gamepad.current;

        if (gamepad == null) return;

        StopCoroutine(Shake());
        StartCoroutine(Shake());

        IEnumerator Shake()
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSecondsRealtime(duration);
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}
