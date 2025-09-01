using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HighLightMaster : MonoBehaviour
{
    public static HighLightMaster instance;

    [SerializeField] RectTransform maskTransform;
    [SerializeField] float showDuration;

    private HighLight baseHighLight;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        baseHighLight = HighLight.RectTransformToHighLight(maskTransform);
    }

    public void SetRect(HighLight target)
    {
        //StartCoroutine(MatchRectTransformCoroutine(target, maskTransform, showDuration));
        AAA(target, maskTransform, showDuration);

    }

    public void Hide()
    {
        //StartCoroutine(MatchRectTransformCoroutine(baseHighLight, maskTransform, showDuration));
        AAA(baseHighLight, maskTransform, showDuration);
    }

    /*
    /// <summary>
    /// 指定したduration秒かけてsourceRectの位置とサイズにtargetRectを変更します。
    /// </summary>
    private IEnumerator MatchRectTransformCoroutine(HighLight highLight, RectTransform targetRect, float duration)
    {
        Vector2 initialPosition = targetRect.anchoredPosition;
        Vector2 initialSize = targetRect.sizeDelta;

        // アニメーションを実行
        for(float f = 0f; f < 1f; f+= Time.unscaledDeltaTime / duration)
        {
            // 線形補間で位置とサイズを更新
            targetRect.anchoredPosition = Vector2.Lerp(initialPosition, highLight.anchordPosition, f);
            targetRect.sizeDelta = Vector2.Lerp(initialSize, highLight.sizeDelta, f);

            yield return null; // 次のフレームまで待機
        }

        // 最終位置とサイズをセット
        targetRect.position = highLight.anchordPosition;
        targetRect.sizeDelta = highLight.sizeDelta;
    }
    */

    private void AAA(HighLight highLight, RectTransform targetRect, float duration)
    {
        Vector2 initialPosition = targetRect.anchoredPosition;
        Vector2 initialSize = targetRect.sizeDelta;

        targetRect.DOAnchorPos(highLight.anchordPosition, duration).SetUpdate(true);
        targetRect.DOSizeDelta(highLight.sizeDelta, duration).SetUpdate(true);
    }
}
[System.Serializable]
public class HighLight
{
    public Vector2 anchordPosition;
    public Vector2 sizeDelta;

    public static HighLight RectTransformToHighLight(RectTransform rectTransform)
    {
        return new HighLight()
        {
            anchordPosition = rectTransform.anchoredPosition,
            sizeDelta = rectTransform.sizeDelta
        };
    }
}
