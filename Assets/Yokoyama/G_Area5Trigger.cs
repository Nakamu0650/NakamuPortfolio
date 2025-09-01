using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G_Area5Trigger : MonoBehaviour
{
    [SerializeField] int enemycount;
    [SerializeField] UnityEvent enemyAllDeathEvent= new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyCounter()
    {
        enemycount--;
        if(enemycount==0)
        {
            enemyAllDeathEvent.Invoke();
        }
    }

}
