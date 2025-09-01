using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class P_LookUpScript : MonoBehaviour
{
    [SerializeField] float lookUpRadius;
    public void OnLookUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var cols = Physics.OverlapSphere(transform.position, lookUpRadius).Where(value=>value.gameObject.CompareTag("ExaminableGimmick")).ToArray();
            if (cols == null) return;
            if (cols.Length > 0)
            {
                cols[0].gameObject.GetComponent<G_ExaminableGimmick>().OnExamined(transform);
            }
        }
    }
}



