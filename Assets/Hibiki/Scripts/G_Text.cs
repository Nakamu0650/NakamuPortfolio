using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Text", menuName = "Hanadayori/Text")]
public class G_Text : ScriptableObject
{
    public SerializedDictionary<Setting.LanguageType, Text[]> texts;

    [Serializable]
    public class Text
    {
        public string speaker;
        public string text;

        public string GetText()
        {
            return $"{speaker}:{text}";
        }
    }

    private void OnValidate()
    {
        foreach(Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
        {
            if (!texts.ContainsKey(language))
            {
                texts.Add(language, new Text[1]);
            }
        }
    }
}
