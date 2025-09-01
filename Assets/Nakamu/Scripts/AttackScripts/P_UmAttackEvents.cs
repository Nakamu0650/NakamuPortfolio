using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_UmAttackEvents : MonoBehaviour
{
    [SerializeField] GameObject umbrellaObj;
    [SerializeField] GameObject roseObj;
    //[SerializeField] SphereCollider sphereCol;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void AttackStart_Um()
    {
        umbrellaObj.SetActive(true);
        //sphereCol.enabled = true;
    }

    void AttackEnd_Um()
    {
        //sphereCol.enabled = false;
        umbrellaObj.SetActive(false);
    }

    void AttackStart_Rose()
    {
        roseObj.SetActive(true);
    }

    void AttackEnd_Rose()
    {
        roseObj.SetActive(false);
    }
}
