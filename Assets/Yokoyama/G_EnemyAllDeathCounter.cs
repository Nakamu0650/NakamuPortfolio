using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G_EnemyAllDeathCounter : MonoBehaviour
{
    //[SerializeField] List<GameObject> enemy = new List<GameObject>();
    [SerializeField] List<F_HP> enemyHP;
    int deathcount;
    [SerializeField] UnityEvent allDeathEvent = new UnityEvent();
    List<bool> isKilled = new List<bool>();
    bool isAllDeath = false;
    // Start is called before the first frame update
    void Start()
    {
        deathcount = 0;
        isAllDeath = false;
        
        for(int i = 0;i<enemyHP.Count;i++)
        {
            isKilled.Add(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i<enemyHP.Count;i++)
        {
            if (isKilled[i] == false && enemyHP[i].isKilled)
            {
                isKilled[i] = true;
                deathcount++;
            }
        }

        if(deathcount == enemyHP.Count)
        {
            if (!isAllDeath)
            {
                Debug.Log("alldeath");
                allDeathEvent.Invoke();
                isAllDeath = true;
            }
        }
    }
}
