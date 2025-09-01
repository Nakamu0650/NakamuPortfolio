using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class G_SceneManager : MonoBehaviour
{
    [SerializeField] TransitionData transitionData = new TransitionData();
    public static G_SceneManager instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transitionData.maskRect.transform.localScale = Vector3.zero;
        transitionData.hideRect.transform.localScale = Vector3.zero;
    }

    public void LoadScene(string sceneName)
    {
        instance.StartCoroutine(Transition());

        IEnumerator Transition()
        {
            transitionData.maskRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.DOScale(transitionData.fadeSize, transitionData.fadeOutDuration).SetEase(transitionData.fadeOutEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeOutDuration);

            SceneManager.LoadScene(sceneName);

            transitionData.maskRect.DOScale(transitionData.fadeSize, transitionData.fadeInDuration).SetEase(transitionData.fadeInEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeInDuration);
        }
    }
    public void LoadScene(int sceneBuildIndex)
    {
        instance.StartCoroutine(Transition());

        IEnumerator Transition()
        {
            transitionData.maskRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.DOScale(transitionData.fadeSize, transitionData.fadeOutDuration).SetEase(transitionData.fadeOutEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeOutDuration);

            SceneManager.LoadScene(sceneBuildIndex);

            transitionData.maskRect.DOScale(transitionData.fadeSize, transitionData.fadeInDuration).SetEase(transitionData.fadeInEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeInDuration);

        }
    }

    public float loadingValue;
    public void LoadSceneWithLoading(string sceneName)
    {
        loadingValue = 0f;
        instance.StartCoroutine(Transition());

        IEnumerator Transition()
        {
            transitionData.maskRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.DOScale(transitionData.fadeSize, transitionData.fadeOutDuration).SetEase(transitionData.fadeOutEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeOutDuration);

            SceneManager.LoadScene(sceneName);

            transitionData.loadingCanvas.DOFade(1f, 0.5f).SetUpdate(true);
            while(loadingValue < 1f)
            {
                transitionData.loadingImage.fillAmount = loadingValue;
                yield return null;
            }
            transitionData.loadingCanvas.DOFade(0f, 0.5f).SetUpdate(true);

            transitionData.maskRect.DOScale(transitionData.fadeSize, transitionData.fadeInDuration).SetEase(transitionData.fadeInEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeInDuration);
        }
    }

    public void LoadSceneWithLoading(int sceneBuildIndex)
    {
        loadingValue = 0f;
        instance.StartCoroutine(Transition());

        IEnumerator Transition()
        {
            transitionData.maskRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.transform.localScale = Vector3.zero;
            transitionData.hideRect.DOScale(transitionData.fadeSize, transitionData.fadeOutDuration).SetEase(transitionData.fadeOutEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeOutDuration);

            SceneManager.LoadScene(sceneBuildIndex);

            transitionData.loadingCanvas.DOFade(1f, 0.5f).SetUpdate(true);
            while (loadingValue < 1f)
            {
                transitionData.loadingImage.fillAmount = loadingValue;
                yield return null;
            }
            transitionData.loadingCanvas.DOFade(0f, 0.5f).SetUpdate(true);

            transitionData.maskRect.DOScale(transitionData.fadeSize, transitionData.fadeInDuration).SetEase(transitionData.fadeInEase).SetUpdate(true);
            yield return new WaitForSecondsRealtime(transitionData.fadeInDuration);
        }
    }

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Serializable]
    private class TransitionData
    {
        public RectTransform maskRect, hideRect;
        public UnityEngine.UI.Image loadingImage;
        public CanvasGroup loadingCanvas;
        public float fadeOutDuration = 1f;
        public float fadeInDuration = 1f;
        public float fadeSize = 1.5f;
        public Ease fadeOutEase = Ease.Linear;
        public Ease fadeInEase = Ease.Linear;
    }
}
