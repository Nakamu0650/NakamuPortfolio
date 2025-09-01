using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_BloomWindEvent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool Active;
    // Start is called before the first frame update
    void Start()
    {
        if (!Active)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActiveWind()
    {
        gameObject.SetActive(true);
    }

    public void NotActiveWind()
    {
        gameObject.SetActive(false);
    }
}
