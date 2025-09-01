using UnityEngine;
using UnityEngine.Events;

public class G_AnimationEventTrigger : MonoBehaviour
{
    public UnityEvent[] onEvents;

    public void Invoke(int number)
    {
        if (onEvents.Length <= number) return;

        onEvents[number].Invoke();
    }
}
