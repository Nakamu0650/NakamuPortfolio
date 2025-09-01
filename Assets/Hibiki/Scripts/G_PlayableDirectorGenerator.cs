using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class G_PlayableDirectorGenerator : MonoBehaviour
{
    public enum PlayMode { None, Onece, All}

    public PlayableDirector playableDirector;

    public PlayMode playMode;

    public Events events;

    [HideInInspector] public bool isPlayed;

    // Start is called before the first frame update
    void Start()
    {
        isPlayed = false;
    }

    public void Play()
    {
        switch (playMode)
        {
            case PlayMode.None:
                {
                    return;
                }
            case PlayMode.Onece:
                {
                    if (!isPlayed)
                    {
                        StartCoroutine(play());
                    }
                    return;
                }
            case PlayMode.All:
                {
                    StartCoroutine(play());
                    return;
                }
        }

        IEnumerator play()
        {
            playableDirector.Play();
            events.onStart.Invoke();

            yield return new WaitUntil(() => playableDirector.state != PlayState.Playing);

            events.onEnd.Invoke();
        }
    }


    [Serializable]
    public class Events
    {
        public UnityEvent onStart, onEnd;
    }
}
