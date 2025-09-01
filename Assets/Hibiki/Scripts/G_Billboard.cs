using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Billboard : MonoBehaviour
{
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(target);
    }
}
