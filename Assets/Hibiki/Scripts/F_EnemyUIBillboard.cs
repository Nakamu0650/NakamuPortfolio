using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class F_EnemyUIBillboard : MonoBehaviour
{
    private Transform target;
    private CanvasGroup canvasGroup;
    private float nowPercent;
    [SerializeField] Slider hpSlider;
    [SerializeField] F_HP hpScript;
    [SerializeField] float damageChangeDuration=1f;
    // Start is called before the first frame update
    void Start()
    {
        hpSlider.value = 1f;
        target = Camera.main.transform;
        canvasGroup = GetComponent<CanvasGroup>();

        hpScript.damagedEvents.AddListener(OnDamage);
        hpScript.killedEvents.AddListener(OnKilled);
    }

    private void FixedUpdate()
    {
        transform.LookAt(target);
    }

    public void OnDamage()
    {
        float beforePercent = hpSlider.value;
        float nowPercent = hpScript.GetHPPercent();

        DOVirtual.Float(beforePercent, nowPercent, damageChangeDuration, value =>
         {
             hpSlider.value = value;
         }).SetEase(Ease.OutSine);
    }

    public void OnKilled()
    {
        OnDamage();
        canvasGroup.DOFade(0f, damageChangeDuration);
    }
}
