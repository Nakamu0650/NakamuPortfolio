using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class G_HaveNoEnergy : MonoBehaviour
{
    [SerializeField] float duration = 2f, fadeDuration = 0.5f;
    [SerializeField] Image flowerImage; 
    private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }


    public void OnNoEnergy(Sprite flower)
    {
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            flowerImage.sprite = flower;
            canvasGroup.DOFade(1f, fadeDuration);
            yield return new WaitForSeconds(fadeDuration + duration);
            canvasGroup.DOFade(0f, fadeDuration);

        }
    }
}
