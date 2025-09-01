using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_TransWreathAttackScript : P_UmbrellaAttackScript
{
    // This is ChildClass about WreathAtack.
    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("TransUmbrellaAtack");
        }
    }
}
