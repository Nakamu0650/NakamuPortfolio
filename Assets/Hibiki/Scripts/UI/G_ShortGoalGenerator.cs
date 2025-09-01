using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_ShortGoalGenerator : MonoBehaviour
{
    public enum Mode { None, Once, All };

    public bool playOnAwake = false;
    public Mode mode = Mode.Once;
    public SerializedDictionary<Setting.LanguageType, string> texts;
    public UnityEngine.Events.UnityEvent onShow, onHide, onChangeProgressBer = new UnityEngine.Events.UnityEvent();

    private bool isPlayed;
    private G_ShortGoalMaster master;
    [SerializeField] float preValue = 0f;
    [SerializeField] bool isComplited = false;

    private void OnValidate()
    {
        foreach(Setting.LanguageType l in System.Enum.GetValues(typeof(Setting.LanguageType)))
        {
            texts.TryAdd(l, "");
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (mode == Mode.None)
        {
            Destroy(this);
        }
        master = G_ShortGoalMaster.instance;
        isPlayed = false;

        yield return null;
        if (playOnAwake)
        {
            Show();
        }
    }

    public void Show()
    {
        if (mode == Mode.Once && isPlayed)
        {
            return;
        }
        isPlayed = true;

        master.SetAndOpen(texts[GameManager.instance.setting.languageSetting.textLanguage]);
        onShow.Invoke();
    }

    public void Hide()
    {
        master.Close();
        onHide.Invoke();
    }

    public void SetProgressBer(float value)
    {
        preValue += value;
        master.SetProgress(preValue);
        
        if(preValue >= 1)
        {
            master.Close();
            preValue = 0;
            onChangeProgressBer.Invoke();
            
        }
    }
}
