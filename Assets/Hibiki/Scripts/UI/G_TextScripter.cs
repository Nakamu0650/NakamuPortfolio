using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class G_TextScripter : MonoBehaviour
{
    public static G_TextScripter instance;

    [SerializeField] GameObject arowObj;
    [SerializeField] float showLetterDuration = 0.1f;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] float goNextAutoDuration = 1f;
    [SerializeField] float shakePower, shakeDuration;
    [SerializeField] int shakeVibrato;
    [SerializeField]public bool autoPlay = false;
    private G_ConversationScript script=new G_ConversationScript();
    private CanvasGroup thisCanvasGroup;
    private RectTransform thisRect;
    [SerializeField] TMP_Text nameText, scriptText;
    private bool goNext;
    [SerializeField] G_TutorialBarContent[] textContents;
    private G_TutorialBarContent[] normalContents;
    [HideInInspector]public bool isShowing;
    [HideInInspector] public int scriptState = -1;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialization
        goNext=false;
        isShowing=false;
        scriptState = -1;

        //GetComponents
        thisCanvasGroup = GetComponent<CanvasGroup>();
        thisRect = GetComponent<RectTransform>();

        LoadScript();
    }

    //Call and Show Conversation.
    public void ShowConversation(string[] showTexts,bool stopTime=false)
    {
        StartCoroutine(ShowConversationEnumrator(showTexts,stopTime));
    }

    //PlayerInput Go next or Show all letters
    public void OnNextButton(InputAction.CallbackContext context)
    {
        if (!isShowing) { return; }
        goNext = context.canceled;
        S_SEManager.PlayTextSE(transform);
    }

    //Get conversation Json to use.
    public G_ConversationScript GetScript()
    {
        return script;
    }

    public void LoadScript()
    {
        //Get json file.
        string languageName = Enum.GetName(typeof(Setting.LanguageType), GameManager.instance.setting.languageSetting.textLanguage);
        string jsonString = Resources.Load<TextAsset>("ConversationScripts/" + languageName).ToString();
        script = JsonUtility.FromJson<G_ConversationScript>(jsonString);
    }

    //ShowConversation
    public IEnumerator ShowConversationEnumrator(string[] showTexts,bool stopTime = false)
    {
        yield return new WaitUntil(() => !isShowing);
        scriptState = -1;
        float _timeScale = Time.timeScale;
        P_UmbrellaAttackManager.canAttack = false;
        G_TutorialBar instance = G_TutorialBar.instance;
        normalContents = instance.GetContents();
        if (stopTime)
        {
            Time.timeScale = 0f;

            instance.ChangeBarContents(textContents);
        }
        else
        {
            foreach (var content in textContents)
            {
                instance.AddContent(content);
            }
        }

        isShowing = true;
        arowObj.SetActive(false);
        thisCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);

        string characterName = "";
        foreach (string showText in showTexts)
        {
            scriptState++;
            var str = AnalyzeScript(showText);
            if (str.characterName != null)
            {
                characterName = str.characterName;
            }
            nameText.text = characterName;
            if (str.doShake)
            {
                thisRect.DOShakeAnchorPos(shakeDuration, shakePower,shakeVibrato);
            }
            for (int i = 0; i < str.script.Length; i++)
            {
                if (goNext)
                {
                    scriptText.text = str.script;
                    goNext = false;
                    break;
                }
                if (str.script[i] == '<')
                {
                    while (str.script[i] != '>')
                    {
                        i++;
                        if ((i+1) == str.script.Length)
                        {
                            break;
                        }
                    }
                }
                scriptText.text = str.script.Substring(0, i + 1);
                yield return new WaitForSecondsRealtime(showLetterDuration);
            }
            arowObj.SetActive(true);
            float _waitTime = 0f;
            while (!goNext && (_waitTime < goNextAutoDuration))
            {
                if (autoPlay) { _waitTime += Time.unscaledDeltaTime; }
                yield return null;
            }
            goNext = false;
            yield return null;
            arowObj.SetActive(false);
        }

        thisCanvasGroup.DOFade(0f, fadeDuration).SetUpdate(true);
        isShowing = false;
        if (stopTime)
        {
            Time.timeScale = _timeScale;
        }
        instance.ChangeBarContents(normalContents);
        P_UmbrellaAttackManager.canAttack = true;
    }

    /// <summary>
    /// Separate into character name and script based on ":".
    /// If ":" is missing, nameText returns null.
    /// If you want to use ":" as a symbol, write "!:"
    /// If there are two or more ":" characters, an error will occur.
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    private (string characterName, string script,bool doShake) AnalyzeScript(string _text)
    {
        _text= _text.Replace("!:", "_RePlAcE_");

        string[] analysisTexts = _text.Split(":");

        if (analysisTexts.Length > 2)
        {
            Debug.LogError("there are two or more \":\" characters");
        }
        for(int i = 0; i < analysisTexts.Length; i++)
        {
            analysisTexts[i]= analysisTexts[i].Replace("_RePlAcE_", ":");
        }
        string name;
        string script;
        if (analysisTexts.Length == 1)
        {
            name = null;
            script = analysisTexts[0];
        }
        else
        {
            name = analysisTexts[0];
            script = analysisTexts[1];
        }

        string[] randomText = script.Split("[RAND]");
        script = randomText[UnityEngine.Random.Range(0, randomText.Length)];

        bool shakable = script.Contains("[SHAKE]");
        script = script.Replace("[SHAKE]", "");
        return (name, script, shakable);

    }
}

/// <summary>
/// Json File class
/// If you want to add conversation, add here.
/// </summary>
[Serializable]
public class G_ConversationScript
{
    public string[]
        changeToWinter1,
        changeToWinter2,
        winterResult1,
        winterResult2,
        test1,
        test2,
        tutorial_blooming,
        tutorial_examine,
        tutorial_objective,
        tutorial_generateWreath,
        tutorial_attackWreath1,
        tutorial_attackWreath2,
        tutorial_attackWreath3,
        tutorial_rose1,
        tutorial_rose2,
        tutorial_sunflower1,
        tutorial_sunflower2,
        tutorial_calendula1,
        tutorial_calendula2,
        tutorial_transformation,
        tutorial_nemophila1,
        tutorial_nemophila2,
        tutorial_hibiscus1,
        tutorial_hibiscus2,
        tutorial_gentian1,
        tutorial_gentian2,
        witch_timeOver,
        witch_killed,
        witch_playerKilled;
}
