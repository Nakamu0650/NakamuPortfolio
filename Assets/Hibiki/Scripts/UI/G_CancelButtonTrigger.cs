using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_CancelButtonTrigger : MonoBehaviour
{
    private GameManager gameManager;

    public void OnCancelButtonClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GetGameManager().OnCancelButton();
        }
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
}
