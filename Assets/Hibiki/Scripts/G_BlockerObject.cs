using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_BlockerObject : MonoBehaviour
{
    private void OnValidate()
    {
        Set();
    }
    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    private void Set()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
}
