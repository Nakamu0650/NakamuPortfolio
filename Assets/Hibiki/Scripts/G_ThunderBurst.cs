using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_ThunderBurst : MonoBehaviour
{
    [SerializeField] F_AttackValue attackValue;
    [SerializeField] F_Param paramater;
    [SerializeField] LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        var cols = Physics.OverlapSphere(transform.position, transform.localScale.x / 2f, playerLayer);
        foreach(var col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.GetComponent<F_HP>().OnDamage(paramater.GetATK(), attackValue.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(transform.position, col.transform.position));
            }
        }
    }
}
