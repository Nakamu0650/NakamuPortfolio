using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class G_InputSystemTrigger : MonoBehaviour
{
    public enum Mode { None, Once, All };
    public InputAction trigger;

    public bool playOnAwake = false;
    public Mode mode = Mode.Once;

    public UnityEngine.Events.UnityEvent onPeformed;

    [HideInInspector] public bool onButton;
    private bool isPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        if (mode == Mode.None)
        {
            Destroy(this);
        }
        trigger.performed += OnButton;
        isPlayed = false;
        onButton = false;
        if (playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        if (mode == Mode.Once && isPlayed)
        {
            return;
        }
        trigger.Enable();
    }

    

    private void OnButton(InputAction.CallbackContext context)
    {
        onButton = true;
        onPeformed.Invoke();
        if(mode == Mode.Once)
        {
            isPlayed = true;
            trigger.Disable();
        }
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            yield return null;
            onButton = false;
        }
    }
}
