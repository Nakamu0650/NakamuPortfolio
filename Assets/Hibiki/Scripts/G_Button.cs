using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(EventTrigger))]
public class G_Button : MonoBehaviour
{
    [SerializeField] List<ChangeColorImage> changeColorImages = new List<ChangeColorImage>();
    [SerializeField] List<ChangeColorTMP_Text> changeColorTMP_Texts = new List<ChangeColorTMP_Text>();

    void Start()
    {
        changeColorImages.ForEach(image =>
        {
            image.image.color = image.transition.normalColor;
        });
        changeColorTMP_Texts.ForEach(text =>
        {
            text.text.color = text.transition.normalColor;
        });
    }

    public void OnPointerEnter()
    {
        changeColorImages.ForEach(image =>
        {
            ChangeImageColor(image.image, image.transition.highlightedColor, image.transition.fadeDuration);
        });
        changeColorTMP_Texts.ForEach(text =>
        {
            ChangeTMPTextColor(text.text, text.transition.highlightedColor, text.transition.fadeDuration);
        });
    }

    public void OnPointerExit()
    {
        changeColorImages.ForEach(image =>
        {
            ChangeImageColor(image.image, image.transition.normalColor, image.transition.fadeDuration);
        });
        changeColorTMP_Texts.ForEach(text =>
        {
            ChangeTMPTextColor(text.text, text.transition.normalColor, text.transition.fadeDuration);
        });
    }

    public void ChangeImageColor(Image image, Color color, float duration)
    {
        image.DOColor(color, duration);
    }

    public void ChangeTMPTextColor(TMP_Text text, Color color, float duration)
    {
        text.DOColor(color, duration);
    }

    [Serializable]
    private class ChangeColorImage
    {
        public Image image;
        public ColorBlock transition;
    }

    [Serializable]
    private class ChangeColorTMP_Text
    {
        public TMP_Text text;
        public ColorBlock transition;
    }
}
