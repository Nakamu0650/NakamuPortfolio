using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_HitStopGenerator : MonoBehaviour
{
    public static G_HitStopGenerator instance;
    [SerializeField] Vector2 stopTime = new Vector2(0f, 0.5f);
    [SerializeField] Vector2Int damageRange = new Vector2Int(0, 100);
    [SerializeField] AnimationCurve timeScaleCurve;
    [HideInInspector] public bool isHitStoping;


    private void Awake()
    {
        instance = this;
        isHitStoping = false;
    }

    /// <summary>
    /// Stop time a few seconds.
    /// </summary>
    /// <param name="damage"></param>
    public void Stop(int damage)
    {
        if (isHitStoping) return;

        float _size = Mathf.InverseLerp(damageRange.x, damageRange.y, damage);

        float _duration = Mathf.Lerp(stopTime.x, stopTime.y, _size);

        StartCoroutine(enumerator());

        IEnumerator enumerator()
        {
            isHitStoping = true;

            float startScale = Time.timeScale;

            for(float f = 0; f < 1f; f += Time.unscaledDeltaTime / _duration)
            {
                Time.timeScale = Mathf.Clamp01(timeScaleCurve.Evaluate(f));
                yield return null;
            }

            Time.timeScale = startScale;

            isHitStoping = false;
        }
    }
}
