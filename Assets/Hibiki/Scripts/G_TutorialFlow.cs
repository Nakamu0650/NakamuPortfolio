using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class G_TutorialFlow : MonoBehaviour
{
    [SerializeField] G_WitchSummoner witchSummoner;

    [SerializeField] UnityEngine.Events.UnityEvent onAnalysisEnd;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        //解析完了まで待機
        yield return new WaitUntil(() => G_FlowerBloomSystem.isAnalysed);
        onAnalysisEnd.Invoke();
    }

    public void OnSetWinter()
    {
        witchSummoner.Summon();
        S_SEManager.SEControl("playerWalk", 0, 0.1f, S_SEManager.playerWalkAisacName[0], 0);
    }

    public void OnClear(Collider collider)
    {
        Clear();
    }

    public void Clear()
    {

        G_SeasonManager.instance.SetSeason(G_SeasonManager.Season.Spring);
        G_SceneManager.instance.LoadScene(0);
    }

    [Serializable]
    private class Conversations
    {
        public G_Text introduction;
    }
}
