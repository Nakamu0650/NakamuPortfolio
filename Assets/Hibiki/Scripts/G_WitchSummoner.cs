using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class G_WitchSummoner : MonoBehaviour
{
    public GameObject witch;
    [SerializeField] Transform player;
    [SerializeField] G_TextScripter scripter;
    [SerializeField] G_SeasonManager seasonManager;
    [SerializeField] G_SkyController skyController;
    private Vector3 playerStartPos;
    private F_HP witchHP;
    private G_BossBase boss;
    private PlayableDirector playableDirector;

    private void Start()
    {
        playerStartPos = player.position;
        witch.GetComponent<G_Lila>().player = player;
        witchHP = witch.GetComponent<F_HP>();
        boss = witch.GetComponent<G_BossBase>();
        playableDirector = GetComponent<PlayableDirector>();
        witch.SetActive(false);
    }

    ///<summary>
    ///Summon Witch in Winter
    /// </summary>
    public void SummonWitch()
    {
        switch (G_SeasonManager.season)
        {
            case G_SeasonManager.Season.Spring:
                {
                    StartCoroutine(Spring());
                    break;
                }
            case G_SeasonManager.Season.Winter:
                {
                    StartCoroutine(Winter());
                    break;
                }
        }
        return;
        IEnumerator Spring()
        {
            yield return StartCoroutine(G_FadeMachine.FadeIn(0.5f));
            yield return StartCoroutine(scripter.ShowConversationEnumrator(scripter.GetScript().winterResult2, true));
            witch.SetActive(false);
            StartCoroutine(G_FadeMachine.FadeOut(0.5f));

        }
        IEnumerator Winter()
        {
            bool hide = (G_SeasonManager.numberOfYears == 0);
            string[] scripts = hide ? scripter.GetScript().changeToWinter1 : scripter.GetScript().changeToWinter2;
            yield return StartCoroutine(scripter.ShowConversationEnumrator(scripts, true));
            Time.timeScale = 0f;
            yield return StartCoroutine(G_FadeMachine.FadeIn(0.5f));
            if (!hide)
            {
                Summon();
            }
            else
            {
                skyController.ChangeSeasonForce(0);
                seasonManager.SetSeason(G_SeasonManager.Season.Spring);
                yield return StartCoroutine(scripter.ShowConversationEnumrator(scripter.GetScript().winterResult1, false));
            }
            player.position = playerStartPos;
            yield return StartCoroutine(G_FadeMachine.FadeOut(0.5f));
            Time.timeScale = 1f;
        }
    }

    public void Summon()
    {
        witchHP.Reset();
        witch.transform.position = transform.position;
        Vector3 direction = (player.position - transform.position);
        witch.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        witch.SetActive(true);
        skyController.ChangeSeasonForce(3);
        seasonManager.SetSeason(G_SeasonManager.Season.Winter,true);
        StartCoroutine(PlayTimeLine());
    }

    public IEnumerator PlayTimeLine()
    {
        Time.timeScale = 0f;
        playableDirector.Play();
        for(float f = 0f; f < playableDirector.duration; f += Time.unscaledDeltaTime)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        boss.OnBattleStart();
    }
}
