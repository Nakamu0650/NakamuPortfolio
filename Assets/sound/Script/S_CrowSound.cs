using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class S_CrowSound : MonoBehaviour
{
    [SerializeField] float CrowRadius = 10f;
    [SerializeField] LayerMask PlayerLayer;
    bool isCrow = false;
    bool isChange = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerFound())
        {
            if (isCrow == false)
            {
                isCrow = true;
            }
        }
        else
        {
            if (isCrow)
            {
                isCrow = false;
            }
        }

        if(isCrow)
        {
            if (!isChange)
            {
                Debug.Log("crow");
                S_SEManager.PlayLila_BardAttackSE(transform);
                isChange = true;
            }
        }
        else
        {
            if(isChange)
            {
                isChange = false;
            }
        }
    }

    void PlayCrowSound()
    {

    }
     
    bool PlayerFound()
    {
        var cols = Physics.OverlapSphere(transform.position, CrowRadius, PlayerLayer).Where(col => col.gameObject.CompareTag("Player"));
        foreach (var col in cols)
        {
            if (col.gameObject == gameObject)
            {
                continue;
            }

            if (col != null)
            {
                isCrow = true;
                return true;
            }
        }
        return false;
    }
}
