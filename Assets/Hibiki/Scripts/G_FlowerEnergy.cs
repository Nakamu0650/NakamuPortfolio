using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class G_FlowerEnergy : MonoBehaviour
{
    public int flowerEnegyAmount=0;
    [SerializeField]public int maxElowerEnegy;
    private Image energyAmountImage;
    private float fillAmount;
    private bool isAmountShowing;
    private bool isColorWhite;
    private int flowerAmount;
    private Vector3 defaultTextSize;
    [SerializeField] Image flowerImage;
    [SerializeField] Image borderImage;
    [SerializeField] CanvasGroup amountImage;
    [SerializeField] TMP_Text amountText;
    [Header("Adjustable values")]
    [SerializeField] public G_Flower.FlowerList flowerType;
    [SerializeField] float textPunchPower = 1f;
    [SerializeField] float textPunchDuration = 1f;
    [SerializeField] int maxShowAmount = 99;
    [SerializeField] float colorChangeSpeed = 0.25f;
    [SerializeField] float fillAmountChangeSpeed = 10f;
    // Start is called before the first frame update
    private void OnValidate()
    {
        energyAmountImage = GetComponent<Image>();
        SetBool();
        ChangeUI(false);
        energyAmountImage.fillAmount = ((float)flowerEnegyAmount / (float)maxElowerEnegy) % 1f;
        defaultTextSize = amountText.rectTransform.localScale;

    }
    void Start()
    {
        energyAmountImage = GetComponent<Image>();
        SetBool();
        ChangeUI(false);
        energyAmountImage.fillAmount = ((float)flowerEnegyAmount / (float)maxElowerEnegy) % 1f;
    }

    private void SetBool()
    {
        isAmountShowing = ((flowerEnegyAmount / maxElowerEnegy) != 0);
        isColorWhite = (flowerEnegyAmount != 0);
    }

    // Update is called once per frame
    void Update()
    {
        fillAmount = Mathf.Lerp(fillAmount, (float)flowerEnegyAmount/(float)maxElowerEnegy, fillAmountChangeSpeed * Time.deltaTime);
        energyAmountImage.fillAmount = fillAmount % 1f;
    }
    public void ChangeUI(bool doTween=true)
    {
        if (doTween)
        {
            int nowFlowerAmount = flowerEnegyAmount / maxElowerEnegy;
            if (flowerAmount != nowFlowerAmount)
            {
                flowerAmount = nowFlowerAmount;
                amountText.text = (flowerAmount <= maxShowAmount ? flowerAmount.ToString() : maxShowAmount + "+");
                amountText.rectTransform.DOComplete();
                amountText.rectTransform.DOPunchScale(textPunchPower * Vector3.one, textPunchDuration)/*.SetEase(Ease.InOutSine)*/;
            }

            bool isEnergyZero = flowerEnegyAmount == 0;
            if (isEnergyZero ^ isColorWhite)
            {
                isColorWhite = !isColorWhite;
                Color _color = isEnergyZero ? Color.black : Color.white;
                flowerImage.DOColor(_color, colorChangeSpeed);
                borderImage.DOColor(_color, colorChangeSpeed);
            }

            bool isFlowerZero = flowerAmount == 0;
            if (isFlowerZero ^ isAmountShowing)
            {
                isAmountShowing = !isAmountShowing;
                amountImage.DOFade(isFlowerZero ? 0f : 1f, colorChangeSpeed);
            }
        }
        else
        {
            int nowFlowerAmount = Mathf.FloorToInt((float)flowerEnegyAmount / (float)maxElowerEnegy);
            flowerAmount = nowFlowerAmount;
            amountText.text = (flowerAmount <= maxShowAmount ? flowerAmount.ToString() : maxShowAmount + "+");

            bool isEnergyZero = flowerEnegyAmount == 0;
            isColorWhite = isEnergyZero;
            Color _color = isEnergyZero ? Color.black : Color.white;
            flowerImage.color = _color;
            borderImage.color = _color;

            bool isFlowerZero = flowerAmount == 0;
            isAmountShowing = !isFlowerZero;
            amountImage.alpha = isFlowerZero ? 0f : 1f;
        }

    }
}
