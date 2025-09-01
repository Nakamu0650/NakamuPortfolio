using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class G_FadeMachine : MonoBehaviour
{
    private static CanvasGroup thisCanvas;

    private void Start()
    {
        thisCanvas = GetComponent<CanvasGroup>();
        thisCanvas.alpha = 0f;
    }

    //hide the screen
    public static IEnumerator FadeIn(float duration,Ease ease=Ease.Linear)
    {
        thisCanvas.DOFade(1f, duration).SetEase(ease).SetUpdate(true);
        yield return new WaitForSecondsRealtime(duration);
    }

    //Showing the hidden screen.
    public static IEnumerator FadeOut(float duration, Ease ease = Ease.Linear)
    {
        thisCanvas.DOFade(0f, duration).SetEase(ease).SetUpdate(true);
        yield return new WaitForSecondsRealtime(duration);
    }
}
