using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "TutorialBarContent", menuName = "Hanadayori/TutorialBarContent")]
public class G_TutorialBarContent : ScriptableObject
{
    public string japaneseName, englishName;
    public G_TutorialBar.ButtonType buttonType;
}
