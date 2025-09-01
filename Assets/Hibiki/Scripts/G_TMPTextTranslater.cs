using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class G_TMPTextTranslater : MonoBehaviour
{
    public SerializedDictionary<Setting.LanguageType, string> texts = new SerializedDictionary<Setting.LanguageType, string>();
    [SerializeField] Setting.LanguageType checkLanguage = Setting.LanguageType.Japanese;

    private TMP_Text text;
    private GameManager gameManager;
    private readonly Dictionary<Setting.LanguageType, string> defaultTexts = new SerializedDictionary<Setting.LanguageType, string>() { { Setting.LanguageType.Japanese, "テキストを入力してください" }, {Setting.LanguageType.English, "Enter Text"} };

    private void OnValidate()
    {
        foreach(Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
        {
            if (!texts.ContainsKey(language))
            {
                texts.Add(language, defaultTexts[language]);
            }
        }

        text = GetComponent<TMP_Text>();
        if (text)
        {
            text.text = texts[checkLanguage];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.onSettingChanged.AddListener(SetText);
        SetText();
    }

    public void SetText()
    {
        if (!text)
        {
            text = GetComponent<TMP_Text>();
            if (!text)
            {
                Debug.LogError("TMP_Textがアタッチされていません", this);
                return;
            }
        }
        text.text = texts[gameManager.setting.languageSetting.textLanguage];
    }

}
