using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class G_TMPTextTranslate : MonoBehaviour
{
    [SerializeField] SerializedDictionary<Setting.LanguageType, string> texts = new SerializedDictionary<Setting.LanguageType, string>();

    private GameManager gameManager;
    private TMP_Text text;

    private void OnValidate()
    {
        foreach(Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
        {
            texts.TryAdd(language, string.Empty);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        text = GetComponent<TMP_Text>();

        gameManager.onSettingChanged.AddListener(OnLanguageChanged);

        OnLanguageChanged();
    }

    public void OnLanguageChanged()
    {
        var language = gameManager.setting.languageSetting.textLanguage;
        text.text = texts[language];
    }
}
