using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_ActiveEnemy : MonoBehaviour
{
    [SerializeField]bool isActive = false;
    [SerializeField]List<GameObject> ActiveObject = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in ActiveObject)
        {
            if (!isActive)
            {
                obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveObjec()
    {
        foreach (GameObject obj in ActiveObject)
        {
            obj.SetActive(true);
        }
    }

    public void NotActiveObject()
    {
        foreach (GameObject obj in ActiveObject)
        {
            obj.SetActive(false);
        }
    }

}
