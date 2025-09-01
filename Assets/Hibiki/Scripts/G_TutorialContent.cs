using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "TutorialContent", menuName = "Hanadayori/TutorialContent"),Serializable]
public class G_TutorialContent : ScriptableObject
{
    public G_TutorialManager.RectValue contentTransform;

    public SerializedDictionary<Setting.LanguageType, string[]> texts = new SerializedDictionary<Setting.LanguageType, string[]>();
    
}
