using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_UmbrellaHoldAttack : MonoBehaviour
{
    public G_Flower.FlowerList useFlower = G_Flower.FlowerList.None;
    public int useEnergy = 3;

    protected P_PlayerMove playerMove;

    private G_FlowerEnergyReceiver energyReceiver;
    // Start is called before the first frame update
    void Start()
    {
        energyReceiver = G_FlowerEnergyReceiver.instance;
        playerMove = P_PlayerMove.instance;
        Set();
    }

    public virtual void Set()
    {

    }

    public virtual void OnAttack()
    {

    }

    public virtual void OnAttackEnd()
    {

    }

    /// <summary>
    /// エネルギーを消費せず攻撃可能かをチェックする
    /// </summary>
    /// <returns></returns>
    public bool CanUseEnergy()
    {
        return energyReceiver.UseEnergy(useFlower, useEnergy, false);
    }

    /// <summary>
    /// エネルギーを消費して攻撃可能かをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool UseEnergy()
    {
        return energyReceiver.UseEnergy(useFlower, useEnergy, true);
    }

    public bool CanAttack()
    {
        return playerMove.GetCanMove();
    }
}
