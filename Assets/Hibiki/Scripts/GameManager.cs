using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public class GameManager : MonoBehaviour
{
    public enum Difficulty {Peacefull = 0 , Easy = 1, Normal = 2, Hard = 3, Master  =4};

    public static GameManager instance;

#if UNITY_EDITOR
    public bool useInsspectorValue = false;
#endif

    public  GameObject menuDefault, settingDefault;
    public Difficulty difficulty = Difficulty.Normal;
    [SerializeField] GameObject gameMenu;
    public Setting setting = new Setting();
    [SerializeField] UI settingUI;
    [SerializeField] EnumArrays enums;
    public UnityEvent onSettingChanged = new UnityEvent();

    private bool isOpeningGameMenu;

    private EventSystem eventSystem;


    //Do this component singleton
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR

            //using Inspector value if "useInsspectorValue" only UnityEditor.
            if (useInsspectorValue)
            {
                SetSetting();
                return;

            }

#endif
            isOpeningGameMenu = false;
            setting = DataRW.Load<Setting>("settingData");
            ShowSetting();
            SetSetting();
            AddApplyEvent();
            DataRW.Save(setting, "settingData");
        }
    }


    public void OpenGameMenu()
    {
        StartCoroutine(openingGameMenu());
    }

    public void CloseGameMenu()
    {
        isOpeningGameMenu = false;

        try
        {
            MainManager.instance.CloseGameMenu();
        }
        catch { }
    }

    public void ReturnToTitle()
    {
        CloseGameMenu();
        S_BGMer.bgmExPlayer.Stop();
        G_SceneManager.instance.LoadScene(0);
    }

    private IEnumerator openingGameMenu()
    {
        gameMenu.SetActive(true);
        isOpeningGameMenu = true;
        SetSelected(menuDefault);

        while (isOpeningGameMenu)
        {
            Time.timeScale = 0f;
            yield return null;
        }

        gameMenu.SetActive(false);
        Time.timeScale = 1f;
    }


    /// <summary>
    /// Reflect changes in settings to other scripts
    /// </summary>
    public void SetSetting()
    {
        S_BGMer.bgmvalue = setting.soundSetting.bgmVolume;
        S_BGMer.sevalue = setting.soundSetting.seVolume;
        S_BGMer.voisevalue = setting.soundSetting.voiceVolume;
        S_BGMer.generalvalue = setting.soundSetting.generalVolume;

        try
        {
            MainManager.instance.ChangeDevice(setting.oparationSetting.useInputDevice);
        }
        catch { }
    }

    /// <summary>
    /// Reflect changes made on the settings screen to settings
    /// </summary>
    private void ApplySetting()
    {
        Setting set = new Setting();
        UI ui = settingUI;

        set.oparationSetting.useInputDevice = enums.inputDevices[ui.useInputDevice.GetIntValue()];
        set.oparationSetting.cameraSensitivity = Vector2.one * ui.cameraSensitivity.GetFloatValue();
        set.oparationSetting.fpsCameraSensitivity = Vector2.one * ui.fpsCameraSensitivity.GetFloatValue();

        set.imageQualitySetting.antiareas = enums.antiareas[ui.antiareas.GetIntValue()];

        set.soundSetting.generalVolume = ui.generalVolume.GetFloatValue();
        set.soundSetting.bgmVolume = ui.bgmVolume.GetFloatValue();
        set.soundSetting.seVolume = ui.seVolume.GetFloatValue();
        set.soundSetting.voiceVolume = ui.voiceVolume.GetFloatValue();

        set.languageSetting.textLanguage = enums.languageTypes[ui.textLanguage.GetIntValue()];
        set.languageSetting.voiceLanguage = enums.languageTypes[ui.voiceLanguage.GetIntValue()];

        set.otherSetting.isMapRotate = ui.isMapRotate.GetBoolValue();
        set.otherSetting.doControlerShake = ui.doControllerShake.GetBoolValue();
        set.otherSetting.doCameraShake = ui.doCameraShake.GetBoolValue();
        set.otherSetting.textAutoPlay = ui.textAutoPlay.GetBoolValue();
        set.otherSetting.textPlaySpeed = ui.textPlaySpeed.GetIntValue();

        setting = set;

        DataRW.Save(set, "settingData");

        onSettingChanged.Invoke();
        SetSetting();
    }

    /// <summary>
    /// Update and apply the settings screen contents to the latest settings
    /// </summary>
    private void ShowSetting()
    {
        UI ui = settingUI;

        ui.useInputDevice.SetIntValue(Array.IndexOf(enums.inputDevices, setting.oparationSetting.useInputDevice));
        ui.cameraSensitivity.SetFloatValue(setting.oparationSetting.cameraSensitivity.magnitude);
        ui.fpsCameraSensitivity.SetFloatValue(setting.oparationSetting.fpsCameraSensitivity.magnitude);

        ui.antiareas.SetIntValue(Array.IndexOf(enums.antiareas, setting.imageQualitySetting.antiareas));

        ui.generalVolume.SetFloatValue(setting.soundSetting.generalVolume);
        ui.bgmVolume.SetFloatValue(setting.soundSetting.bgmVolume);
        ui.seVolume.SetFloatValue(setting.soundSetting.seVolume);
        ui.voiceVolume.SetFloatValue(setting.soundSetting.voiceVolume);

        ui.textLanguage.SetIntValue(Array.IndexOf(enums.languageTypes, setting.languageSetting.textLanguage));
        ui.voiceLanguage.SetIntValue(Array.IndexOf(enums.languageTypes, setting.languageSetting.voiceLanguage));

        ui.isMapRotate.SetBoolValue(setting.otherSetting.isMapRotate);
        ui.doControllerShake.SetBoolValue(setting.otherSetting.doControlerShake);
        ui.doCameraShake.SetBoolValue(setting.otherSetting.doCameraShake);
        ui.textAutoPlay.SetBoolValue(setting.otherSetting.textAutoPlay);
        ui.textPlaySpeed.SetIntValue(setting.otherSetting.textPlaySpeed);
    }


    /// <summary>
    /// Automatically reflect the event that the settings are updated when the value on the settings screen is updated
    /// </summary>
    private void AddApplyEvent()
    {
        Type type = settingUI.GetType();

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach(FieldInfo field in fields)
        {
            if (field.FieldType == typeof(G_Settings))
            {
                var setting = (G_Settings)field.GetValue(settingUI);
                setting.onValueChanged.AddListener(() => ApplySetting());
            }
        }
    }

    public void CancelButtonInvoke(GameObject selectUIObject, GameObject hidenObject = null, GameObject showObject = null)
    {
        SetSelected(selectUIObject);

        if (hidenObject != null)
        {
            hidenObject.SetActive(false);
        }
        if (showObject != null)
        {
            showObject.SetActive(true);
        }
    }

    [HideInInspector] public bool cancelButton = false;

    public void WaitCancelButton(GameObject selectUIObject, GameObject hidenObject = null, GameObject showObject = null)
    {
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            cancelButton = false;
            yield return new WaitUntil(() => cancelButton);
            cancelButton = false;
            CancelButtonInvoke(selectUIObject, hidenObject, showObject);

        }
    }

    public void OnCancelButton()
    {
        cancelButton = true;
    }


    public void SetSelected(GameObject selectObject)
    {
        StartCoroutine(select());
        IEnumerator select()
        {
            yield return null;
            Debug.Log(selectObject, selectObject);
            GetEventSystem().SetSelectedGameObject(selectObject);
        }
    }

    public GameObject GetSelected()
    {
        

        return GetEventSystem().currentSelectedGameObject;
    }

    public EventSystem GetEventSystem()
    {
        if (!eventSystem)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            if (!eventSystem)
            {
                Debug.LogError("EventSystem is not found.", this);
                return null;
            }
        }
        return eventSystem;
    }


    [Serializable]
    public class CanvasActivate
    {
        public List<Pair> canvases = new List<Pair>();

        public void Setcanvas(int num)
        {
            for (int i = 0; i < canvases.Count; i++)
            {
                canvases[i].canvas.SetActive(num == i);
            }
            instance.SetSelected(canvases[num].select);
        }

        [Serializable]
        public class Pair
        {
            public GameObject canvas;
            public GameObject select;
        }
    }


    [Serializable]
    private class UI
    {
        [Header("Oparation")]
        public G_Settings useInputDevice;
        public G_Settings cameraSensitivity;
        public G_Settings fpsCameraSensitivity;

        [Header("Image Quality")]
        public G_Settings antiareas;

        [Header("Sound")]
        public G_Settings generalVolume;
        public G_Settings bgmVolume;
        public G_Settings seVolume;
        public G_Settings voiceVolume;

        [Header("Language")]
        public G_Settings textLanguage;
        public G_Settings voiceLanguage;

        [Header("Other")]
        public G_Settings isMapRotate;
        public G_Settings doControllerShake;
        public G_Settings doCameraShake;
        public G_Settings textAutoPlay;
        public G_Settings textPlaySpeed;
    }

    [Serializable]
    private class EnumArrays
    {
        public Setting.InputDevice[] inputDevices;
        public Setting.Antiareas[] antiareas;
        public Setting.LanguageType[] languageTypes;
    }
}


namespace EggSystem
{
    [Serializable]
    public class Gaps
    {
        public float before { get { return beforeValue; } set { beforeValue = Mathf.Max(value, 0f); } }
        public float affect { get { return affectValue; } set { affectValue = Mathf.Max(value, 0f); } }
        public float after { get { return afterValue; } set { afterValue = Mathf.Max(value, 0f); } }

        [SerializeField] float beforeValue;
        [SerializeField] float affectValue;
        [SerializeField] float afterValue;

        public Gaps(float _before, float _affect, float _after)
        {
            before = _before;
            affect = _affect;
            after = _after;
        }
    }

    [Serializable]
    public class Range<T> where T : struct
    {
        public T Min
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }
        public T Max
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

        [SerializeField] T m_minValue;
        [SerializeField] T m_maxValue;

        public Range(T minValue, T maxValue)
        {
            m_minValue = minValue;
            m_maxValue = maxValue;
        }


        public void ForEach(Func<T, T> action)
        {
            m_minValue = action(m_minValue);
            m_maxValue = action(m_maxValue);
        }

        public Range<T> Select(Func<T, T> action)
        {
            return new Range<T>(action(m_minValue), action(m_maxValue));
        }
    }
    public class Gizmo
    {
        public static void DrawCapsuleGizmo(Vector3 start, Vector3 end, float radius)
        {
            var preMatrix = Gizmos.matrix;

            // カプセル空間（(0, 0)からZ軸方向にカプセルが伸びる空間）からワールド座標系への変換行列
            Gizmos.matrix = Matrix4x4.TRS(start, Quaternion.FromToRotation(Vector3.forward, end), Vector3.one);

            // 球体を描画
            var distance = (end - start).magnitude;
            var capsuleStart = Vector3.zero;
            var capsuleEnd = Vector3.forward * distance;
            Gizmos.DrawWireSphere(capsuleStart, radius);
            Gizmos.DrawWireSphere(capsuleEnd, radius);

            // ラインを描画
            var offsets = new Vector3[] { new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f) };
            for (int i = 0; i < offsets.Length; i++)
            {
                Gizmos.DrawLine(capsuleStart + offsets[i] * radius, capsuleEnd + offsets[i] * radius);
            }

            Gizmos.matrix = preMatrix;
        }
    }
}


[Serializable]
public class Setting
{
    //Declare a class to be included in Setting
    [Serializable]
    public class Oparation
    {
        public InputDevice useInputDevice = InputDevice.KeyboadAndMouse;
        public Vector2 cameraSensitivity = Vector2.one;
        public Vector2 fpsCameraSensitivity = Vector2.one * 0.25f;
    }

    [Serializable]
    public class ImageQuality
    {
        public Antiareas antiareas = Antiareas.TAA;
    }

    [Serializable]
    public class Sound
    {
        [Range(0f,1f)]public float generalVolume = 1f,bgmVolume = 1f, seVolume = 1f, voiceVolume = 1f;
    }

    [Serializable]
    public class Language
    {
        public LanguageType textLanguage = LanguageType.Japanese;
        public LanguageType voiceLanguage = LanguageType.Japanese;
    }

    [Serializable]
    public class Other
    {
        public bool isMapRotate = false;
        public bool doControlerShake = true;
        public bool doCameraShake = true;
        public bool textAutoPlay = false;
        [Range(1, 5)] public int textPlaySpeed = 3;
    }


    //Declare variables to be actually used
    public Oparation oparationSetting = new Oparation();
    public ImageQuality imageQualitySetting = new ImageQuality();
    public Sound soundSetting = new Sound();
    public Language languageSetting = new Language();
    public Other otherSetting = new Other();


    //Enumerated type set
    public enum InputDevice { Gamepad, KeyboadAndMouse};
    public enum LanguageType { Japanese, English };
    public enum Antiareas { Off, FXAA, TAA};
}

[Serializable]
public class DifficultyValue<T>
{
    public SerializedDictionary<GameManager.Difficulty, T> values = new SerializedDictionary<GameManager.Difficulty, T>();

    public T GetValue()
    {
        var difficulty = GameManager.instance.difficulty;

        if (values.ContainsKey(difficulty))
        {
            return values[difficulty];
        }
        else
        {
            return default(T);
        }
    }

    public void SetAll()
    {
        foreach (GameManager.Difficulty difficulty in Enum.GetValues(typeof(GameManager.Difficulty)))
        {
            values.TryAdd(difficulty, default(T));
        }
    }

    public DifficultyValue(params T[] difficultyValues)
    {
        int i = 0;
        foreach(GameManager.Difficulty d in Enum.GetValues(typeof(GameManager.Difficulty)))
        {
            if(difficultyValues.Length <= i)
            {
                values[d] = default(T);
            }
            else
            {
                values[d] = difficultyValues[i];
            }
            i++;
        }
    }
}

[Serializable]
public class LanguageValue<T>
{
    public SerializedDictionary<Setting.LanguageType, T> values = new SerializedDictionary<Setting.LanguageType, T>();

    public T GetValue()
    {
        var difficulty = GameManager.instance.setting.languageSetting.textLanguage;

        if (values.ContainsKey(difficulty))
        {
            return values[difficulty];
        }
        else
        {
            return default(T);
        }
    }

    public void SetAll()
    {
        foreach(Setting.LanguageType language in Enum.GetValues(typeof(Setting.LanguageType)))
        {
            values.TryAdd(language, default(T));
        }
    }
}
