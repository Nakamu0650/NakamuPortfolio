using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class G_TutorialMovieGenerator : MonoBehaviour
{
    [SerializeField] G_TutorialWithMovie.TutorialContent content;

    private G_TutorialWithMovie tutorial;
    private bool isShowing, isShowed;

    private void OnValidate()
    {
        foreach (Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
        {
            if (!content.text.ContainsKey(language))
            {
                content.text.Add(language, "");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isShowing = false;
        isShowed = false;
        tutorial = G_TutorialWithMovie.instance;
    }

    public void Show()
    {
        if(isShowing || isShowed)
        {
            return;
        }
        isShowed = true;
        isShowing = true;
        tutorial.ShowTutorial(content);
    }

    public void Hide()
    {
        if (!isShowing)
        {
            return;
        }

        tutorial.HideTutorial();
    }
}
