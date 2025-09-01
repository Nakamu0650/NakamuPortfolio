using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Searcher : MonoBehaviour
{
    [HideInInspector]public Transform player;
    [HideInInspector]public bool isPunish;
    [SerializeField]public UnityEvent onPunish = new UnityEvent(),onLoss = new UnityEvent();
    public static UnityEvent onPuniched = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        isPunish = false;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player = col.transform;
            isPunish = true;
            onPunish.Invoke();
            onPuniched.Invoke();
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isPunish = false;
            onLoss.Invoke();
        }
    }
}
