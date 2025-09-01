using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class G_ExaminableGimmicUI : MonoBehaviour
{
    [SerializeField] G_TutorialBarContent examineBarContent;
    [SerializeField] CanvasGroup imageCanvasGroup;
    [SerializeField] float fadeSecond = 0.25f, punchSecond = 0.5f;

    private RectTransform imageRect;
    private bool inPlayer;
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        imageRect = imageCanvasGroup.GetComponent<RectTransform>();

        imageCanvasGroup.alpha = 0f;

        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inPlayer) { return; }
        imageRect.LookAt(cameraTransform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inPlayer = true;
            G_TutorialBar.instance.AddContent(examineBarContent);
            imageCanvasGroup.DOFade(1f, fadeSecond);
            imageRect.localScale = Vector3.zero;
            imageRect.DOScale(1f, punchSecond).SetEase(Ease.OutElastic);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inPlayer = false;
            G_TutorialBar.instance.RemoveContent(examineBarContent.buttonType);
            imageCanvasGroup.DOFade(0f, fadeSecond);
        }
    }

}
