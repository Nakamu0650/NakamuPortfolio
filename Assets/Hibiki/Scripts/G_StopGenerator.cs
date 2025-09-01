using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_StopGenerator : MonoBehaviour
{
    public enum Mode { None, Once, All };

    [HideInInspector] public bool onStart = false;

    public bool playOnAwake = false;
    public Mode mode = Mode.Once;
    public float timeScale = 0f;
    public UnityEngine.Events.UnityEvent onPlay, onEnd;

    private bool isPlayed;

    // Start is called before the first frame update
    void Start()
    {
        if(mode == Mode.None)
        {
            Destroy(this);
        }
        onStart = false;
        isPlayed = false;
    }

    public void Stop()
    {
        onStart = true;
    }

    public void Play()
    {
        if (mode == Mode.Once && isPlayed)
        {
            return;
        }
        StartCoroutine(PlayCoroutine());
    }

    public IEnumerator PlayCoroutine()
    {
        if (mode == Mode.Once && isPlayed)
        {
            yield break;
        }
        isPlayed = true;
        float beforeTimeScale = Time.timeScale;
        onPlay.Invoke();
        while (!onStart)
        {
            Time.timeScale = timeScale;
            yield return null;
        }
        Time.timeScale = beforeTimeScale;
        onEnd.Invoke();
    }


}
