using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class G_ShortGoalMaster : MonoBehaviour
{
    public static G_ShortGoalMaster instance;
    [SerializeField] TMP_Text goalText;
    [SerializeField] Slider progressSlider;
    [SerializeField] float openAndCloseDuration = 1f;
    [SerializeField] float updateProgressBerDuration = 0.5f;
    [SerializeField] Ease openAndCloseEase;
    [SerializeField] Ease updateProgressEase;

    private float showXPos;
    private RectTransform thisRect;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        showXPos = thisRect.rect.size.x;
        Reset();
    }

    public void SetAndOpen(string text)
    {
        Set(text);
        Open();
    }

    public void Set(string text)
    {
        goalText.text = text;
        progressSlider.value = 0f;
    }

    public void Open()
    {
        thisRect.DOAnchorPosX(0f, openAndCloseDuration).SetEase(openAndCloseEase).SetUpdate(true);
    }

    public void Close()
    {
        thisRect.DOAnchorPosX(showXPos, openAndCloseDuration).SetEase(openAndCloseEase).SetUpdate(true);
    }

    public void SetProgress(float value)
    {
       
       DOVirtual.Float(progressSlider.value, value, updateProgressBerDuration, f =>
       {
            progressSlider.value = f;
       }).SetEase(updateProgressEase).SetUpdate(true);
    }

    private void Reset()
    {
        goalText.text = "";
        progressSlider.value = 0f;
        thisRect.anchoredPosition = new Vector2(showXPos, thisRect.anchoredPosition.y);
    }
}
