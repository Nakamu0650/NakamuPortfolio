using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class G_PoisonicEnergyUI : MonoBehaviour
{
    [SerializeField] Image potImage;
    [SerializeField] CanvasGroup buttonsCanvasGroup;
    [SerializeField] float changeDuration = 0.25f;
    [SerializeField] Ease changeEase;

    [SerializeField] float notFullAlpha = 0.5f;

    private CanvasGroup thisCanvasGroup;
    private float beforeValue;
    // Start is called before the first frame update
    void Start()
    {
        thisCanvasGroup = GetComponent<CanvasGroup>();
        beforeValue = 0f;
        SetPercent(beforeValue);
    }

    public void SetPercent(float value)
    {
        beforeValue = value;
        float alpha = Alpha(value);
        float buttonAlpha = ButtonAlpha(value);
        thisCanvasGroup.alpha = alpha;
        buttonsCanvasGroup.alpha = buttonAlpha;
        potImage.fillAmount = value;
    }

    public void FixEnergy(float value)
    {
        float alpha = Alpha(value);
        float buttonAlpha = ButtonAlpha(value);
        thisCanvasGroup.DOFade(alpha, changeDuration).SetEase(changeEase).SetUpdate(true);
        buttonsCanvasGroup.DOFade(buttonAlpha, changeDuration).SetEase(changeEase).SetUpdate(true);
        potImage.DOFillAmount(value, changeDuration).SetEase(changeEase).SetUpdate(true);
    }

    private float Alpha(float f) => ((f == 1)?1f:notFullAlpha);
    private float ButtonAlpha(float f) => ((f == 1) ? 1f : 0f);
}
