using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Setting;

public class G_Select : MonoBehaviour
{
    private G_SettingsController controller;
    public string sceneName;
    [SerializeField] GameManager.CanvasActivate canvasActivate;
    [SerializeField] InputAction specialActionModifire;
    S_SoundManager soundManager;

    private GameManager gameManager;
    private bool isSpecialAction;

    // Start is called before the first frame update
    void Start()
    {
        isSpecialAction = false;
        specialActionModifire.performed += OnSpecialModifire;
        specialActionModifire.canceled += OnSpecialModifire;
        specialActionModifire.Enable();

        soundManager = S_SoundManager.Instance;
        gameManager = GameManager.instance;
        SetCanvas(0);
    }

    public void OnDifficultyButtonClick(int difficultyNumber)
    {
        GameManager.Difficulty difficulty = (GameManager.Difficulty)Enum.ToObject(typeof(GameManager.Difficulty), difficultyNumber);
        GameManager.instance.difficulty = difficulty;
        S_SEManager.PlayStartgameSE(transform);
        soundManager.Stop("menubgm");
        G_SceneManager.instance.LoadSceneWithLoading(sceneName);

    }

    public void OnSettingButtonClick()
    {
        GetSettingController().OpenSetting();
        S_SEManager.PlayStartSelectSE(transform);
        soundManager.Stop(S_BGMer.titlebgmName);
    }

    public void OnEndButtonClick()
    {
        if (isSpecialAction)
        {
            Restart();
        }
        else
        {
            End();
        }
    }

    private void Restart()
    {
        Debug.Log("再起動");
#if UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
        Application.Quit();
#else
        End();
#endif
    }

    private void End()
    {
        Debug.Log("終了");
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }

    public void OnCloseCredit(InputAction.CallbackContext context)
    {
        if (context.performed && canvasActivate.canvases[2].canvas.activeSelf)
        {
            SetCanvas(0);
        }
    }

    private G_SettingsController GetSettingController()
    {
        if (!controller)
        {
            controller = G_SettingsController.instance;

            if (!controller)
            {
                Debug.LogError("SettingsController does not exist", this);
                return null;
            }
        }

        return controller;
    }

    private GameManager GetGameManager()
    {
        if (!gameManager)
        {
            gameManager = GameManager.instance;
            if (!gameManager)
            {
                Debug.LogError("GameManager does not exist", this);
                return null;
            }
        }
        return gameManager;
    }

    public void SetCanvas(int num)
    {
        canvasActivate.Setcanvas(num);
    }

    private void OnSpecialModifire(InputAction.CallbackContext context)
    {
        if (isSpecialAction)
        {
            isSpecialAction = !context.canceled;
        }
        else
        {
            isSpecialAction = context.performed;
        }
    }
}
