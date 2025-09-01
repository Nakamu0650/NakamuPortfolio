using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

public class G_FlowerEnergyReceiver : MonoBehaviour
{
    public static G_FlowerEnergyReceiver instance;
#if UNITY_EDITOR
    [SerializeField] bool debugMode = false;
#endif
    public SerializedDictionary<G_Flower.FlowerList, FlowerEnergy> flowerEnergies = new SerializedDictionary<G_Flower.FlowerList, FlowerEnergy>();
    private G_BuffSystem buffs;
    public ReceiveEvent onGetEnergy;
    public ReceiveEvent onUseEnergy;
    [SerializeField] G_HaveNoEnergy haveNoEnergy;
    bool isGet = false;
    public void GetedEnegyEvent()
    {
        isGet = true;
    }


    [Serializable]
    public class FlowerEnergy
    {
        public LanguageValue<string> names;
        public Sprite flowerSprite;
        public Gradient flowerColor;
        public int maxEnergyAmount = 10;
        public int energyAmount = 0;

        public float GetEnergyPercent()
        {
            float p = energyAmount % maxEnergyAmount;
            return (p / (float)maxEnergyAmount);
        }

        public int GetCorollaAmount()
        {
            return (energyAmount / maxEnergyAmount);
        }

        public bool HaveWreath()
        {
            return (energyAmount >= maxEnergyAmount);
        }

        public bool HaveEnergy()
        {
            return (energyAmount > 0);
        }
    }

    [Serializable]
    public class ReceiveEvent : UnityEvent<G_Flower.FlowerList> { }

    private void OnValidate()
    {
        foreach (G_Flower.FlowerList flower in Enum.GetValues(typeof(G_Flower.FlowerList)))
        {
            if (!flowerEnergies.ContainsKey(flower))
            {
                flowerEnergies.Add(flower, new FlowerEnergy());
            }
            flowerEnergies[flower].names.SetAll();
        }
    }

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        buffs = GetComponent<G_BuffSystem>();
    }

#if UNITY_EDITOR

    private void Update()
    {
        SetUnlimitedFlower();
    }
# endif

    public void GetEnergy(G_Flower.FlowerList flowerType, int amount)
    {
        float buffSize = buffs.GetValue(G_BuffSystem.BuffType.Defence);
        int newAmount = Mathf.Clamp(flowerEnergies[flowerType].energyAmount + Mathf.RoundToInt(buffSize * amount), 0, 9999);
        flowerEnergies[flowerType].energyAmount = newAmount;
        if (!isGet)
        {
            onGetEnergy.Invoke(flowerType);
        }

        S_SEManager.PlayGetEnergy(transform);

    }

    public bool UseEnergy(G_Flower.FlowerList flowerType, int amount, bool consume = true)
    {
        if (flowerEnergies[flowerType].energyAmount >= amount)
        {
            if (consume)
            {
                flowerEnergies[flowerType].energyAmount -= amount;
                onUseEnergy.Invoke(flowerType);
            }
            return true;
        }
        else
        {
            S_SEManager.PlaylackOfEnegy2(transform);
            haveNoEnergy.OnNoEnergy(flowerEnergies[flowerType].flowerSprite);
            return false;
        }
    }

    public bool HaveWreath(G_Flower.FlowerList flowerType)
    {
        if (flowerEnergies.ContainsKey(flowerType))
        {
            var energy = flowerEnergies[flowerType];

            return energy.HaveWreath();
        }
        else
        {
            haveNoEnergy.OnNoEnergy(flowerEnergies[flowerType].flowerSprite);
            return true;
        }
    }


    public int EnergyAmount(G_Flower.FlowerList flowerType)
    {
        return flowerEnergies[flowerType].energyAmount;
    }

    public int GetCorollaAmount(G_Flower.FlowerList flowerType)
    {
        return flowerEnergies[flowerType].GetCorollaAmount();
    }

    public float GetEnergyPercent(G_Flower.FlowerList flowerType)
    {
        return flowerEnergies[flowerType].GetEnergyPercent();
    }

    public FlowerEnergy GetFlowerEnergy(G_Flower.FlowerList flowerType)
    {
        return flowerEnergies[flowerType];
    }

#if UNITY_EDITOR
    void SetUnlimitedFlower()
    {
        if (!debugMode) return;
        foreach(var flower in flowerEnergies)
        {
            flower.Value.energyAmount = 99999;
        }
    }
#endif
}