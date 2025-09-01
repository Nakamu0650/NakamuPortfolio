using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class P_PlayerHPBar : MonoBehaviour
{
    [SerializeField] F_HP playerHp;
    [SerializeField] Slider slider;
    [SerializeField] AnimationCurve changeCurve;
    [SerializeField] float duration = 1f;
    [SerializeField] float maxShakeStrength = 100f;
    [SerializeField] int vibratoAmount = 30;

    private float beforeHp;
    private RectTransform thisRect;

    // Start is called before the first frame update
    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        playerHp.damagedEvents.AddListener(OnValueChanged);
        playerHp.healedEvents.AddListener(OnValueChanged);
        beforeHp = 1f;
        slider.value = 1f;
    }


    /// <summary>
    /// Called when player damaged or healed.
    /// </summary>
    public void OnValueChanged()
    {
        float nowHp = playerHp.GetHPPercent();
        StartCoroutine(change());

        IEnumerator change()
        {
            float difference = beforeHp - nowHp;
            if (difference > 0)
            {
                Shake(difference);
            }
            for (float f = 0; f < 1f; f+=Time.unscaledDeltaTime / duration)
            {
                slider.value = Mathf.Lerp(beforeHp, nowHp, changeCurve.Evaluate(f));
                yield return null;
            }
            slider.value = nowHp;
            beforeHp = nowHp;
        }

        void Shake(float strength)
        {
            thisRect.DOComplete();
            thisRect.DOShakeAnchorPos(duration, strength * maxShakeStrength, vibratoAmount);
        }
    }
}
