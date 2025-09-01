using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using System;

public class P_PlayerSelectAttackWreath : MonoBehaviour
{
    [SerializeField] G_FlowerEnergyReceiver flowerEnergyReceiver;
    [SerializeField] G_Flower.FlowerList initialSelectFlower;
    [SerializeField]public G_Flower.FlowerList[] correspondingFlowers = new G_Flower.FlowerList[3];
    [SerializeField] List<P_WreathAttack_Base> wreathes = new List<P_WreathAttack_Base>();
    public UnityEvent onChangeAttackWreath = new UnityEvent();
    [HideInInspector] public G_Flower.FlowerList selectingFlower;

    public static P_PlayerSelectAttackWreath instance;

    

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        selectingFlower = initialSelectFlower;

        int flowerNumber = Array.IndexOf(correspondingFlowers, selectingFlower);
        for (int i = 0; i < wreathes.Count; i++)
        {
            wreathes[i].isActive = (i == flowerNumber);
        }

    }

    public void SelectFlower(int flowerNumber)
    {
        selectingFlower = correspondingFlowers[flowerNumber];

        for (int i = 0; i < wreathes.Count; i++)
        {
            wreathes[i].isActive = (i == flowerNumber);
        }
        onChangeAttackWreath.Invoke();
    }

    public bool UseEnergy(G_Flower.FlowerList _flower,int value)
    {
        bool use = flowerEnergyReceiver.UseEnergy(_flower, value);

        if (use)
        {
        }

        return use;
    }

}
