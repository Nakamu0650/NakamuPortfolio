using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class G_TutorialManager : MonoBehaviour
{
    public static G_TutorialManager instance;
    [SerializeField] RectTransform highLightArea;

    [SerializeField] SerializedDictionary<List<G_TutorialContent>> contentsDictionary;
    private CanvasGroup highLightAreaCanvas;


    [SerializeField] float moveDuration = 0.25f;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        highLightAreaCanvas = highLightArea.GetComponent<CanvasGroup>();
        highLightAreaCanvas.alpha = 0f;
    }

    public void Tutorial(string contentString)
    {
        StartCoroutine(TutorialEnumerator(contentString));
    }

    public IEnumerator TutorialEnumerator(string contentString)
    {
        var scripter = G_TextScripter.instance;
        G_TutorialContent[] contents;
        try
        {
            contents = contentsDictionary[contentString].ToArray();
        }
        catch
        {
            Debug.LogError($"\"{contentString}\" does not exist.");
            yield break;
        }

        RectValue.ValueToTransform(highLightArea, contents[0].contentTransform);
        highLightAreaCanvas.DOFade(1f, moveDuration).SetUpdate(true);

        bool autoPlay = scripter.autoPlay;
        var language = GameManager.instance.setting.languageSetting.textLanguage;

        scripter.autoPlay = false;
        float timeScale = Time.timeScale;
        Time.timeScale = 0f;

        foreach (var content in contents)
        {
            RectValue before = RectValue.TransformToValue(highLightArea);
            DOVirtual.Float(0f, 1f, moveDuration, (value) =>
            {
                RectValue rectValue = RectValue.Lerp(before, content.contentTransform, value);
                RectValue.ValueToTransform(highLightArea, rectValue);
            }).SetEase(Ease.OutSine).SetUpdate(true);

            yield return StartCoroutine(scripter.ShowConversationEnumrator(content.texts[language]));
        }

        Time.timeScale = (timeScale != 0f) ? timeScale : 1f;
        scripter.autoPlay = autoPlay;

        highLightAreaCanvas.DOFade(0f, moveDuration).SetUpdate(true);
    }

    
    [Serializable]
    public class RectValue
    {
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;

        public static RectValue Lerp(RectValue before, RectValue after, float t)
        {
            Vector2 position = Vector2.Lerp(before.anchoredPosition, after.anchoredPosition, t);
            Vector2 sizeDelta = Vector2.Lerp(before.sizeDelta, after.sizeDelta, t);

            RectValue rect = new RectValue
            {
                anchoredPosition = position,
                sizeDelta = sizeDelta
            };

            return rect;
        }

        public static void ValueToTransform(RectTransform transform, RectValue value)
        {
            transform.anchoredPosition = value.anchoredPosition;
            transform.sizeDelta = value.sizeDelta;
        }

        public static RectValue TransformToValue(RectTransform transform)
        {
            RectValue value = new RectValue
            {
                anchoredPosition = transform.anchoredPosition,
                sizeDelta = transform.sizeDelta
            };

            return value;
        }
    }
}
