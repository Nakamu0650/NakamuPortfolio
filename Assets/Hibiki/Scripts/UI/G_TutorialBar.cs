using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

public class G_TutorialBar : MonoBehaviour
{
    public enum ButtonType { LStick, RStick, L1, L2, L3, R1, R2, R3 ,East, West, North, South, Right, Left, Up, Down, Select, Start};

    [SerializeField] G_ControllerType controllerType;
    [SerializeField] List<G_TutorialBarContent> tutorialBarContents;
    [SerializeField] float space = 10f;

    private TMP_Text[] texts;
    private Image[] images;
    private RectTransform[] childrenRects;

    private Dictionary<ButtonType, Sprite> buttonTextures;

    public static G_TutorialBar instance;

    private int length;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        length = Enum.GetValues(typeof(ButtonType)).Length;
        texts = new TMP_Text[length];
        images = new Image[length];
        childrenRects = new RectTransform[length];

        int childCount = transform.childCount;

        if (length < childCount)
        {
            Debug.LogError($"Child count is too many.Set count {length}.(Now is {childCount})");
            return;
        }
        else if (length > childCount)
        {
            Debug.LogError($"Child count is too little.Set count {length}.(Now is {childCount})");
            return;
        }

        for (int i = 0; i < length; i++)
        {
            Transform child = transform.GetChild(i);
            
            images[i] = child.GetChild(0).GetComponent<Image>();
            texts[i] = child.GetChild(1).GetComponent<TMP_Text>();
            childrenRects[i] = child.GetComponent<RectTransform>();
        }
        ChangeControllerType(controllerType);
    }

    public void ChangeControllerType(G_ControllerType newControllerType)
    {
        controllerType = newControllerType;
        buttonTextures = new Dictionary<ButtonType, Sprite>();
        foreach(G_ControllerType.ControllerPathTexture path in controllerType.controllerPathes)
        {
            buttonTextures.Add(path.buttonType, path.texture);
        }
        UpdateBar();
    }

    public void ChangeBarContents(G_TutorialBarContent[] newContents)
    {
        tutorialBarContents = newContents.ToList();

        UpdateBar();
    }

    public G_TutorialBarContent[] GetContents()
    {
        return tutorialBarContents.ToArray();
    }

    public void UpdateBar()
    {
        Setting.LanguageType type = GameManager.instance.setting.languageSetting.textLanguage;
        for (int i = 0; i < length; i++)
        {
            bool enable = (i < tutorialBarContents.Count);

            childrenRects[i].gameObject.SetActive(enable);
            if (enable)
            {
                G_TutorialBarContent content = tutorialBarContents[i];
                if (buttonTextures[content.buttonType])
                {
                    images[i].sprite = buttonTextures[content.buttonType];
                }

                switch (type)
                {
                    case Setting.LanguageType.Japanese:
                        {
                            texts[i].SetText(content.japaneseName);
                            break;
                        }
                    case Setting.LanguageType.English:
                        {
                            texts[i].SetText(content.englishName);
                            break;
                        }
                }
                float height = childrenRects[i].rect.height;
                float textWidth = texts[i].preferredWidth;

                float newWidth = height + textWidth + space;
                childrenRects[i].sizeDelta = new Vector2(newWidth, height);
            }
        }
    }

    public void RemoveContent(ButtonType buttonType)
    {
        foreach(var content in tutorialBarContents)
        {
            if(content.buttonType == buttonType)
            {
                tutorialBarContents.Remove(content);
                UpdateBar();
                return;
            }
        }
    }

    public void AddContent(G_TutorialBarContent content)
    {
        tutorialBarContents.Add(content);
        UpdateBar();
    }
}
