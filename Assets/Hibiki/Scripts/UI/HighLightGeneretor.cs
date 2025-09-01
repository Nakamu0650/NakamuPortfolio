using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightGeneretor : MonoBehaviour
{
    public enum Mode { None, Once, All };

    [HideInInspector] public bool onStop = false;

    public HighLight target;
    public bool playOnAwake = false;
    public Mode mode = Mode.Once;
    public UnityEngine.Events.UnityEvent onPlay, onEnd;

    private bool isPlayed;
    private HighLightMaster master;

    // Start is called before the first frame update
    void Start()
    {
        if (mode == Mode.None)
        {
            Destroy(this);
        }
        onStop = false;
        isPlayed = false;
        master = HighLightMaster.instance;
    }

    public void Stop()
    {
        onStop = true;
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
        master.SetRect(target);
        isPlayed = true;
        onPlay.Invoke();

        yield return new WaitUntil(() => onStop);

        master.Hide();
        onEnd.Invoke();
    }


}
