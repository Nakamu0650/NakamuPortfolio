using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(G_StopGenerator), typeof(HighLightGeneretor), typeof(G_InputSystemTrigger))]
public class G_StopTutorialGenerator : MonoBehaviour
{
    public enum Mode { None, Once, All };
    [Header("ここをいじる")]
    [SerializeField] InputAction trigger = new InputAction("PressToProceed", InputActionType.Button, "<Keyboard>/A");
    [SerializeField] HighLight target;
    [Space(10)]
    [Header("おまけ")]
    public bool playOnAwake = false;
    public Mode mode = Mode.Once;

    public UnityEngine.Events.UnityEvent onPlay, onCompleted;

    private Components components;
    private bool isPlayed;

    // Start is called before the first frame update
    void Start()
    {
        if (mode == Mode.None)
        {
            Destroy(GetComponent<G_StopGenerator>());
            Destroy(GetComponent<HighLightGeneretor>());
            Destroy(GetComponent<G_InputSystemTrigger>());
            Destroy(this);
        }
        Set();
        isPlayed = false;

        if (playOnAwake)
        {
            IEnumerator enumerator()
            {
                yield return null;
                Play();
            }
            StartCoroutine(enumerator());
        }
    }

    public void Play()
    {
        if (mode == Mode.Once && isPlayed)
        {
            return;
        }
        isPlayed = true;
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            onPlay.Invoke();
            yield return StartCoroutine(components.PlayCo());
            onCompleted.Invoke();
        }
    }

    private void Set()
    {
        components = new Components(GetComponent<G_StopGenerator>(), GetComponent<HighLightGeneretor>(), GetComponent<G_InputSystemTrigger>());
        components.Set(trigger, target);
    }

    private class Components
    {
        public G_StopGenerator stopGenerator;
        public HighLightGeneretor highLightGeneretor;
        public G_InputSystemTrigger inputSystemTrigger;

        public Components(G_StopGenerator stop, HighLightGeneretor highLight, G_InputSystemTrigger trigger)
        {
            stopGenerator = stop;
            highLightGeneretor = highLight;
            inputSystemTrigger = trigger;
        }

        public void Set(InputAction inputAction, HighLight highLight)
        {
            inputSystemTrigger.trigger = inputAction;
            highLightGeneretor.target = highLight;
            stopGenerator.timeScale = 0f;
            inputSystemTrigger.mode = G_InputSystemTrigger.Mode.All;
            highLightGeneretor.mode = HighLightGeneretor.Mode.All;
            stopGenerator.mode = G_StopGenerator.Mode.All;
            inputSystemTrigger.playOnAwake = false;
            highLightGeneretor.playOnAwake = false;
            stopGenerator.playOnAwake = false;
        }

        public IEnumerator PlayCo()
        {
            stopGenerator.Play();
            highLightGeneretor.Play();
            inputSystemTrigger.Play();
            yield return new WaitUntil(() => inputSystemTrigger.onButton);
            stopGenerator.Stop();
            highLightGeneretor.Stop();
        }
    }
}
