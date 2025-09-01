using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    /// <summary>
    /// PlayerParameter
    /// </summary>
    [SerializeField] P_PlayerParam playerParam;
    private int HP, maxHP;
    private int DEF, maxDEF;

    private float hpBar;
    private RectTransform rect;
    [SerializeField] Image[] images = new Image[5];
    private bool isKilled;


    [SerializeField] P_DamageManager damageManager;
    [SerializeField] F_Param enemyParam;
    int _str = 10;
    // Start is called before the first frame update
    void Start()
    {
        maxHP = playerParam.HP;
        HP = maxHP;
        maxDEF = playerParam.DEF;
        DEF = maxDEF;
        hpBar = maxHP;
        rect = GetComponent<RectTransform>();
        isKilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar = Mathf.Lerp(hpBar, (float)HP, Time.deltaTime * 10f);

        for (int i = 0; i < images.Length; i++)
        {
            images[i].fillAmount = Mathf.Clamp01(5f * (hpBar / (float)maxHP) - i);
        }
    }

    public void OnDamage(int ATK, int STR)
    {
        if (isKilled) return;
        var _damage = damageManager.DamageCalculation(ATK, STR, DEF);
        damageManager.CurrentDamage = _damage;
        HP -= damageManager.CurrentDamage;
        if (HP <= 0)
        {
            HP = 0;
            isKilled = true;
        }
        rect.DOShakeAnchorPos(0.5f, (float)damageManager.CurrentDamage, 30, 90, false, true);
        foreach (Image image in images)
        {
            image.DOColor(Color.red, 0.25f);
            image.DOColor(Color.white, 0.25f).SetDelay(0.25f);
        }
    }
}