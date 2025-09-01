using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class G_AttackFlowerPots : G_PotsUI
{
    [SerializeField] Energy[] energies = new Energy[3];
    [SerializeField] TMP_Text nameText;
    private P_PlayerSelectAttackWreath attackWreath;

    private List<G_Flower.FlowerList> flowerNumbers;

    private int centerNumber;
    // Set is called before the first frame update
    public override void Set()
    {
        base.Set();
        attackWreath = P_PlayerSelectAttackWreath.instance;

        flowerNumbers = attackWreath.correspondingFlowers.ToList();
        centerNumber = Mathf.RoundToInt((float)flowerNumbers.Count / 2f);

        attackWreath.onChangeAttackWreath.AddListener(OnChangeAttack);

        SetPots();
    }

    public void OnChangeAttack()
    {
        SetPots();
    }

    private void SetPots()
    {
        int selectNum = flowerNumbers.IndexOf(attackWreath.selectingFlower);

        int count = flowerNumbers.Count;
        for (int i = 0; i < count; i++)
        {
            int num = ReapNumber(i + selectNum + centerNumber);
            energies[i].flower = flowerNumbers[num];
            energies[i].pot.SetFlower(receiver, energies[i].flower);
            var energy = energies[i];
            if(!energyPots.TryAdd(energy.flower, energy.pot))
            {
                energyPots[energy.flower] = energy.pot;
            }
        }
        nameText.text = receiver.flowerEnergies[attackWreath.selectingFlower].names.GetValue();
    }

    private int ReapNumber(int num)
    {
        int count = flowerNumbers.Count;
        bool ok = false;
        while (!ok)
        {
            ok = true;
            if (num < 0)
            {
                ok = false;
                num += count;
            }
            else if (num >= count)
            {
                ok = false;
                num -= count;
            }
        }
        return num;
    }


    [Serializable]
    private class Energy
    {
        public G_Flower.FlowerList flower;
        public G_FlowerEnergyPot pot;
    }

}
