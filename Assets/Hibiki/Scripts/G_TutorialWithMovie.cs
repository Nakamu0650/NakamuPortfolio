using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class G_TutorialWithMovie : MonoBehaviour
{
    public static G_TutorialWithMovie instance;

    [SerializeField] RectTransform rect;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] TMP_Text text;
    [SerializeField] SerializedDictionary<TutorialContent> tutorialContents = new SerializedDictionary<TutorialContent>();

    [SerializeField] float showDuration, hideDuration;

    private Vector2 hideAnchordPosition, showAnchordPosision;
    private GameManager gameManager;

    private void OnValidate()
    {
        foreach(var content in tutorialContents)
        {
            foreach (Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
            {
                if (!content.Value.text.ContainsKey(language))
                {
                    content.Value.text.Add(language, "");
                }
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        showAnchordPosision = rect.anchoredPosition;
        hideAnchordPosition = showAnchordPosision + Vector2.right * rect.rect.width;

        rect.anchoredPosition = hideAnchordPosition;

    }

    public void ShowTutorial(string key)
    {
        ShowTutorial(tutorialContents[key]);
    }

    public void ShowTutorial(TutorialContent content)
    {
        ChangeContent(content);
        rect.DOAnchorPos(showAnchordPosision, showDuration).SetEase(Ease.OutSine).SetUpdate(true);
        videoPlayer.Play();
    }

    private void ChangeContent(TutorialContent content)
    {
        videoPlayer.clip = content.videoClip;
        text.text = content.text[gameManager.setting.languageSetting.textLanguage];
    }

    public void HideTutorial()
    {
        rect.DOAnchorPos(hideAnchordPosition, hideDuration).SetEase(Ease.InSine).SetUpdate(true);
        videoPlayer.Stop();
    }

    [Serializable]
    public class TutorialContent
    {
        public VideoClip videoClip;
        public SerializedDictionary<Setting.LanguageType, string> text;
    }
}
