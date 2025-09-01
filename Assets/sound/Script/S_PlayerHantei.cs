using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class S_PlayerHantei : MonoBehaviour
{
    [HideInInspector] public Transform player;
    [SerializeField] float Radius = 15f;
    [SerializeField] LayerMask enemyLayer;
    public static bool isBGMChange = false;
    public enum BGMState
    {
        Battle,
        Field,
    }
    public static BGMState bgmstate = BGMState.Field;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(EnemyFound())
        {
            if(bgmstate == BGMState.Field)
            {
                bgmstate = BGMState.Battle;
            }
        }
        else
        {
            if (bgmstate == BGMState.Battle)
            {
                bgmstate = BGMState.Field;
            }
        }

        
    }

    bool EnemyFound()
    {
        var cols = Physics.OverlapSphere(transform.position, Radius, enemyLayer).Where(col => col.gameObject.CompareTag("Enemy"));
        foreach (var col in cols)
        {
            if (col.gameObject == gameObject)
            {
                continue;
            }
            
            if(col != null)
            {
                return true;
            }
        }
        return false;
    }

   
}
