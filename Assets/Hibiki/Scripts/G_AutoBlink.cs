using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_AutoBlink : MonoBehaviour
{
    [SerializeField] float[] blinkBlendShape;
    [SerializeField] Vector2 blinkDuration;
    [SerializeField] Vector2 blinkSpan;
    private G_BlendShapeChanger blendChanger;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        blendChanger = GetComponent<G_BlendShapeChanger>();
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkSpan.x, blinkSpan.y));
            yield return StartCoroutine(Blink(Random.Range(blinkDuration.x, blinkDuration.y)));
        }
    }

    private IEnumerator Blink(float duration)
    {
        blendChanger.PunchBlendShapeAsNew(blinkBlendShape, duration, DG.Tweening.Ease.Linear);
        yield return new WaitForSeconds(duration);
    }

    //private void OnDisable()
    //{
    //    StopAllCoroutines();
    //}
}
