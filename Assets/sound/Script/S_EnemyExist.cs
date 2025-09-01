using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class S_EnemyExist : MonoBehaviour
{
    public static bool isBGMChange = false;
    List<GameObject> crows = new List<GameObject>();
    List<bool> iscrows = new List<bool>();
    [SerializeField] float targetDistance = 5;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("changebgm");
            isBGMChange = true;
            if (other.GetComponent<G_DependentCrow>())
            {
                crows.Add(other.gameObject);
                iscrows.Add(true);
                StartCoroutine(CheckCrows());
            }
            
            isBGMChange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!other.GetComponent<G_DependentCrow>())
            {
                isBGMChange = false;
            }
        }
    }

    IEnumerator CheckCrows()
    {
        for (int i = 0; i < crows.Count; i++)
        {
            float distance = (transform.position - crows[i].transform.position).sqrMagnitude;
            if (distance < targetDistance * targetDistance)
            {
                if (iscrows[i])
                {
                    S_SEManager.PlayCrowVoiceSE(crows[i].transform);
                    iscrows[i] = false;
                }
            }
        }
        yield return null;
    }
}
