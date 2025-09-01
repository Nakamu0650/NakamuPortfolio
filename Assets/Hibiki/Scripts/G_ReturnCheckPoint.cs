using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class G_ReturnCheckPoint : MonoBehaviour
{
    [SerializeField] bool addOnAwake = false;
    public Events events;

    private P_PlayerMove player;

    // Start is called before the first frame update
    void Start()
    {
        player = P_PlayerMove.instance;
        if (addOnAwake)
        {
            Add();
        }
    }


    public void Add()
    {
        if (!player.returnPoints.Contains(transform))
        {
            player.returnPoints.Add(transform);
        }
    }
    public void Remove()
    {
        if (player.returnPoints.Contains(transform))
        {
            player.returnPoints.Remove(transform);
        }
    }

    [Serializable]
    public class Events
    {
        public UnityEvent onAdd, onRemove;
    }
}
