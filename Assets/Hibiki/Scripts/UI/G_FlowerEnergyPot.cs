using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class G_FlowerEnergyPot : MonoBehaviour
{

    [SerializeField] Image energyImage, flowerImage;
    [SerializeField] RectTransform amountRect;
    [SerializeField] TMP_Text amountText;
    [SerializeField] Durations durations;
    public bool useAmount = true;

    private G_GradientImage gradientImage;
    private CanvasGroup amountCanvasGroup;
    private bool haveEnergy = false;
    private bool haveCorolla = false;
    private int beforeCorollaAmount = 0;
    private float beforeEnergyFill;

    private readonly float amountRectRotateDistance = 360f / Mathf.PI;
    private float amountRectMoveDistance;

    [Serializable]
    private class Durations
    {
        public float showAmount, hideAmount, showEnergy, hideEnergy, fixEnergyAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent();

        amountRectMoveDistance = amountRect.rect.width / 2f;
    }


    public void FixEnergy(G_FlowerEnergyReceiver receiver, G_Flower.FlowerList flower)
    {
        var energy = receiver.flowerEnergies[flower];
        FixEnergy(energy.GetCorollaAmount(), energy.GetEnergyPercent());
    }

    public void FixEnergy(int corollaAmount, float energyFill)
    {
        if (!useAmount)
        {
            energyFill += corollaAmount;
            corollaAmount = 0;
        }

        bool _haveEnergy = (corollaAmount + energyFill != 0f);
        bool _haveCorolla = (corollaAmount != 0);
        float _energyAmount = corollaAmount + energyFill;

        //Get Energy First
        if (!haveEnergy && _haveEnergy)
        {
            haveEnergy = true;
            flowerImage.DOColor(Color.white, durations.showEnergy).SetUpdate(true);
        }
        //Lose All Corolla
        if (haveEnergy && !_haveEnergy)
        {
            haveEnergy = false;
            flowerImage.DOColor(Color.black, durations.hideEnergy).SetUpdate(true);
        }

        //Get Corolla First
        if (!haveCorolla && _haveCorolla)
        {
            haveCorolla = true;

            var showSequence = DOTween.Sequence().SetUpdate(true);
            showSequence.Complete();
            showSequence.Append(amountRect.DORotate(Vector3.zero, durations.showAmount).SetEase(Ease.OutBack))
                .Join(amountRect.DOAnchorPos(Vector2.zero, durations.showAmount).SetEase(Ease.OutBack))
                .Join(amountCanvasGroup.DOFade(1f, durations.showAmount / 2f));
        }

        //Lose All Corolla 
        if (haveCorolla && !_haveCorolla)
        {
            haveCorolla = false;

            var hideSequence = DOTween.Sequence().SetUpdate(true);
            hideSequence.Kill(true);
            hideSequence.Append(amountRect.DORotate(new Vector3(0f, 0f, amountRectRotateDistance), durations.hideAmount).SetEase(Ease.OutBack))
                .Join(amountRect.DOAnchorPos(new Vector2(-amountRectMoveDistance, 0f), durations.hideAmount).SetEase(Ease.OutBack))
                .Join(amountCanvasGroup.DOFade(0f, durations.hideAmount / 2f));
        }

        float beforeEnergyAmount = beforeCorollaAmount + beforeEnergyFill;
        DOVirtual.Float(beforeEnergyAmount, _energyAmount, durations.fixEnergyAmount,f =>
        {
            int i = beforeCorollaAmount;
            beforeCorollaAmount = Mathf.FloorToInt(f);
            beforeEnergyFill = f - beforeCorollaAmount;

            if (i != beforeCorollaAmount)
            {
                amountText.transform.DOComplete();
                amountText.transform.DOPunchScale(Vector3.one, durations.fixEnergyAmount);
            }

            amountText.text = ToCorollaAmountShowing(beforeCorollaAmount);
            energyImage.fillAmount = beforeEnergyFill;
        }
        ).SetEase(Ease.OutSine);
    }

    public void SetAmount(G_FlowerEnergyReceiver receiver, G_Flower.FlowerList flower)
    {
        var energy = receiver.flowerEnergies[flower];
        SetAmount(energy.GetCorollaAmount(), energy.GetEnergyPercent());
    }

    public void SetAmount(int corollaAmount, float energyFill)
    {
        if (!useAmount)
        {
            energyFill += corollaAmount;
            corollaAmount = 0;
        }
        bool _haveEnergy = (corollaAmount + energyFill != 0f);
        bool _haveCorolla = (corollaAmount != 0);

        haveCorolla = _haveCorolla;
        haveEnergy = _haveEnergy;

        beforeCorollaAmount = corollaAmount;
        beforeEnergyFill = energyFill;


        flowerImage.color = haveEnergy ? Color.white : Color.black;
        energyImage.fillAmount = energyFill;
        amountRect.anchoredPosition = haveCorolla ? Vector3.zero : new Vector2(-amountRectMoveDistance, 0f);
        amountRect.eulerAngles = haveCorolla ? Vector3.zero : new Vector3(0f, 0f, amountRectRotateDistance);
        amountCanvasGroup.alpha = haveCorolla ? 1f : 0f;
        amountText.text = (corollaAmount > 99) ? "99+" : corollaAmount.ToString();
    }

    public void SetFlower(G_FlowerEnergyReceiver receiver, G_Flower.FlowerList flower)
    {
        var energy = receiver.flowerEnergies[flower];
        SetFlower(energy.flowerColor, energy.flowerSprite, energy.GetCorollaAmount(), energy.GetEnergyPercent());
    }

    public void SetFlower(Gradient flowerGradient, Sprite flowerSprite, int corollaAmount, float energyFill)
    {
        GetComponent();
        gradientImage.SetGradient(flowerGradient);
        flowerImage.sprite = flowerSprite;

        SetAmount(corollaAmount, energyFill);
    }

    private void GetComponent()
    {
        if (!gradientImage)
        {
            gradientImage = energyImage.gameObject.GetComponent<G_GradientImage>();
        }
        if (!amountCanvasGroup)
        {
            amountCanvasGroup = amountRect.GetComponent<CanvasGroup>();
        }
    }

    public static string ToCorollaAmountShowing(int corollaAmount)
    {
        return (corollaAmount > 99) ? "99+" : corollaAmount.ToString();
    }
}
