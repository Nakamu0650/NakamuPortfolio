using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] string playerAction = "Player";
    [SerializeField] string uiAction = "UI";
    private static readonly Dictionary<Setting.InputDevice, string> schemes = new Dictionary<Setting.InputDevice, string>()
    {
        { Setting.InputDevice.KeyboadAndMouse, "Keyboard&Mause" }, { Setting.InputDevice.Gamepad, "Gamepad" }
    };

    private GameManager gameManager;

    private void Awake()
    {
        instance = this;
        //playerInput.SwitchCurrentControlScheme(schemes[GameManager.instance.setting.oparationSetting.useInputDevice]);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput.actions.FindActionMap(playerAction).Enable();
        playerInput.actions.FindActionMap(uiAction).Disable();

        gameManager = GameManager.instance;
    }

    public void OpenGameMenu()
    {
        playerInput.actions.FindActionMap(playerAction).Disable();
        playerInput.actions.FindActionMap(uiAction).Enable();
        gameManager.OpenGameMenu();
    }

    public void CloseGameMenu()
    {
        playerInput.actions.FindActionMap(playerAction).Enable();
        playerInput.actions.FindActionMap(uiAction).Disable();
    }

    public void ChangeDevice(Setting.InputDevice device)
    {
        playerInput.SwitchCurrentControlScheme(schemes[device]);
    }
}
