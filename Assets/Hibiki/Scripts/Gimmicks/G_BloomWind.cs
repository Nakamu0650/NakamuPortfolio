using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_BloomWind : MonoBehaviour
{
    [SerializeField] float strength = 1f;
    [SerializeField] float minMatchValue = 0.7f;
    private G_BuffSystem player;
    private Transform playerTransform;
    private bool playerInto;
    private void Start()
    {
        playerInto = false;
    }
    private void FixedUpdate()
    {
        if (!player) return;
        if (playerInto)
        {
            float matchValue = Mathf.Clamp01(Vector3.Dot(playerTransform.forward, transform.forward));
            if (matchValue >= minMatchValue)
            {
                int id;
                player.AddBuff(G_BuffSystem.BuffType.Defence, new G_Buff("BloomWindEnergyBuff", out id, strength, Time.fixedDeltaTime));
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            if (!player)
            {
                player = other.gameObject.GetComponent<G_BuffSystem>();
            }
            playerInto = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInto = false;
        }
    }
}
