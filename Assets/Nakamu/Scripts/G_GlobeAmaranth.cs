using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class G_GlobeAmaranth : G_Item_Base
{
    [SerializeField] GameObject healEffectPrefab;
    [SerializeField] DifficultyValue<int> useEnergyAmount;
    [SerializeField] protected F_HP playerHp;
    [SerializeField] G_FlowerEnergyPot pot;
    [SerializeField] UnityEvent recoveryEvent;

    private void OnValidate()
    {
        foreach(GameManager.Difficulty d in Enum.GetValues(typeof(GameManager.Difficulty)))
        {
            useEnergyAmount.values.TryAdd(d, 0);
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        receiver.onGetEnergy.AddListener(OnGetEnergy);
        pot.SetFlower(receiver, flower);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void OnPressButton()
    {
        if (playerHp.HP == playerHp.maxHP) return;
        if (!UseEnergy(useEnergyAmount.GetValue())) return;
        Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
        S_SEManager.PlayRecoverySE(transform);
        recoveryEvent.Invoke();
        base.OnPressButton();
        playerHp.OnHeal(itemParam.effectSize);
        var energy = receiver.flowerEnergies[flower];
        pot.FixEnergy(energy.GetCorollaAmount(), energy.GetEnergyPercent());
    }

    public void OnGetEnergy(G_Flower.FlowerList _flower)
    {
        if (_flower != flower) return;

        var energy = receiver.flowerEnergies[_flower];
        pot.FixEnergy(energy.GetCorollaAmount(), energy.GetEnergyPercent());
    }
}
