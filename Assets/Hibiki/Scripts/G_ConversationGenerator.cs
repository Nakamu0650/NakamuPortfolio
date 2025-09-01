using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class G_ConversationGenerator : MonoBehaviour
{
    private enum CameraType { None, UntilTalking, Duration}

    public G_Text text;
    [SerializeField] bool playOnAwake = false;
    [SerializeField] CameraType cameraType = CameraType.None;
    [SerializeField] GameObject cameraObject;
    [SerializeField] float duration = 0f;
    [SerializeField] bool repeat = false;
    [SerializeField] UnityEvent onConversationStart, onConversationEnd;

    private G_TextScripter scripter;
    private GameManager gameManager;

    private bool isExecuted;
    // Start is called before the first frame update
    void Start()
    {
        isExecuted = false;

        gameManager = GameManager.instance;
        scripter = G_TextScripter.instance;

        if (cameraObject)
        {
            cameraObject.SetActive(false);
        }
        else
        {
            cameraType = CameraType.None;
        }

        if (playOnAwake)
        {
            Talk();
        }
    }

    public void Talk()
    {
        if (isExecuted && !repeat)
        {
            return;
        }
        isExecuted = true;
        var texts = text.texts[gameManager.setting.languageSetting.textLanguage].Select(text => text.GetText()).ToArray();

        onConversationStart.Invoke();
        switch (cameraType)
        {
            case CameraType.None:
                {
                    StartCoroutine(Talking(texts));
                    return;
                }
            case CameraType.UntilTalking:
                {
                    StartCoroutine(TalkingUntil(texts));
                    return;
                }
            case CameraType.Duration:
                {
                    StartCoroutine(TalkingDuration(texts));
                    return;
                }
        }
    }

    private IEnumerator Talking(string[] texts)
    {
        yield return StartCoroutine(scripter.ShowConversationEnumrator(texts));

        onConversationEnd.Invoke();
    }

    private IEnumerator TalkingUntil(string[] texts)
    {
        cameraObject.SetActive(true);
        yield return StartCoroutine(scripter.ShowConversationEnumrator(texts));
        onConversationEnd.Invoke();
        cameraObject.SetActive(false);
    }

    private IEnumerator TalkingDuration(string[] texts)
    {
        StartCoroutine(Talking(texts));
        cameraObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        cameraObject.SetActive(false);
    }
}
