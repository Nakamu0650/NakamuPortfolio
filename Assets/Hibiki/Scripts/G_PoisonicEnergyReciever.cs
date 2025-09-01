using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class G_PoisonicEnergyReciever : MonoBehaviour
{
    public static G_PoisonicEnergyReciever instance;

    [SerializeField] G_PoisonicEnergyUI pot;
    [SerializeField] int maxEnergyAmount;
    [SerializeField] int startEnergyAmount = 0;
    [SerializeField] CanvasGroup potCanvas, buttonUICanvas;
    [SerializeField] private int energyAmount;

    private bool couldUse;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        energyAmount = startEnergyAmount;
        couldUse = false;
        pot.SetPercent(EnergyPercent());
        SetUI();
    }

    /// <summary>
    /// エネルギーを指定の範囲内で加算する
    /// </summary>
    /// <param name="amount">加算する量(負の数の場合減算される)</param>
    public void GetEnergy(int amount)
    {
        energyAmount = Mathf.Clamp((energyAmount + amount), 0, maxEnergyAmount);
        pot.FixEnergy(EnergyPercent());
        SetUI();
    }


    /// <summary>
    /// 指定量のエネルギーを消費できる時にtrueを返す
    /// </summary>
    /// <param name="use">実際に消費する場合はtrueにする</param>
    /// <returns></returns>
    public bool UseEnergy(int amount, bool use = false)
    {
        int newAmount = (energyAmount - amount);
        if (newAmount < 0)
        {
            return false;
        }
        else
        {
            if (use)
            {
                energyAmount = newAmount;
                pot.FixEnergy(EnergyPercent());
                SetUI();
            }
            return true;
        }
    }

    /// <summary>
    /// 必殺技を発動できる時にtrueを返す
    /// </summary>
    /// <param name="use">実際に消費する場合はtrueにする</param>
    /// <returns></returns>
    public bool UseSkill(bool use = false)
    {
        if(maxEnergyAmount == energyAmount)
        {
            if (use)
            {
                energyAmount = 0;
                pot.FixEnergy(EnergyPercent());
                SetUI();
            }
            return true;
        }
        return false;
    }

    private void SetUI()
    {
        bool useSkill = UseSkill();
        if (couldUse ^ useSkill)
        {
            couldUse = useSkill;
            potCanvas.DOFade(useSkill ? 1f : 0.5f, 0.5f);
            buttonUICanvas.DOFade(useSkill ? 1f : 0f, 0.5f);
        }
    }

    /// <summary>
    /// 現在のエネルギーの数を返す
    /// </summary>
    /// <returns></returns>
    public int GetEnergyAmount()
    {
        return energyAmount;
    }

    /// <summary>
    /// 最大値と比較した時のパーセンテージを返す
    /// </summary>
    /// <returns></returns>
    public float GetEnergyPercent()
    {
        return ((float)energyAmount / (float)maxEnergyAmount);
    }

    private float EnergyPercent() => (float) energyAmount / (float) maxEnergyAmount;
}
