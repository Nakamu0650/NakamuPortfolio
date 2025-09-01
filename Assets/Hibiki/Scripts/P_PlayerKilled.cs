using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class P_PlayerKilled : MonoBehaviour
{
    public enum KillEvent { None, GoToTitle, GoToNextSeason}

    public KillEvent killEvent;
    [SerializeField] GoTitle goTitle;

    private GameManager gameManager;
    private G_SceneManager sceneManager;
    private G_SeasonManager seasonManager;
    private P_PlayerMove playerMove;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        sceneManager = G_SceneManager.instance;

        playerMove = P_PlayerMove.instance;

        goTitle.gameOverPanel.alpha = 0f;
        goTitle.gameOverPanel.gameObject.SetActive(false);
    }

    public void OnKilledPlayer()
    {
        GoToTitle();
    }

    public void GoToTitle()
    {
        StartCoroutine(go());
        IEnumerator go()
        {
            playerMove.canMove = false;
            playerMove.canJump = false;
            playerMove.isKinematic = true;
            playerMove.modelAnimator.SetBool(goTitle.isKilledAnimation, true);
            yield return new WaitForSeconds(goTitle.mortionWaitDuration);
            goTitle.gameOverPanel.gameObject.SetActive(true);
            goTitle.gameOverPanel.DOFade(1f, goTitle.panelShowingDuration);
            yield return new WaitForSeconds(goTitle.goTitleDuration);
            S_BGMer.bgmExPlayer.Stop();
            sceneManager.LoadScene(0);
        }
    }


    [Serializable]
    private class GoTitle
    {
        public CanvasGroup gameOverPanel;
        public string isKilledAnimation;
        public float mortionWaitDuration, panelShowingDuration, goTitleDuration;
    }
}
