using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class G_TriggerWater : MonoBehaviour
{
    [SerializeField] LayerMask waterLayer;
    [SerializeField] string tagName = "Water";
    [SerializeField] UnityEvent onTriggerWater;


    //Collider Initialization
    private void OnValidate()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.excludeLayers = ~waterLayer;
        collider.includeLayers = waterLayer;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagName))
        {
            Debug.Log(other.gameObject.name, this);
            onTriggerWater.Invoke();
        }
    }
}
