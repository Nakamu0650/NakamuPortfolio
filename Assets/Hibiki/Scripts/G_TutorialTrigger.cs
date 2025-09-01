using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G_TutorialTrigger : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GameObject gimmicks;
    [SerializeField] G_WitchSummoner witchSummoner;
    [SerializeField] G_TextScripter textScripter;
    [SerializeField] G_SeasonManager seasonManager;
    [SerializeField] G_SkyController skyController;
    [SerializeField] G_SeasonClock seasonClock;
    [SerializeField] TutorialStrings tutorialStrings;
    [SerializeField] G_Flower.FlowerList[] explaneFlowers;
    [SerializeField] string titleSceneName;
    [SerializeField] int sendWreathAmount = 3;
    [SerializeField] float aroundTime = 180f;
    [SerializeField] float againstWitchTime = 90f;

    private Vector3 playerStartPositipon;


    private bool[] isExplaneAttacks = new bool[3];
    private F_HP playerHp;
    private F_HP witchHp;
    private G_TutorialManager tutorialManager;
    private bool explanedAnimal;

    private Dictionary<G_Flower.FlowerList, bool> explanedFlowers;

    private int explaneAttckWreathPhase = 0;

    private int phaseNum;

    private enum Arrive
    {
        arrive0,
        arrive1,
        arrive2,
    }

    private Arrive arrive = Arrive.arrive0;

    private readonly List<G_Flower.FlowerList> transformFlowers = new List<G_Flower.FlowerList> { G_Flower.FlowerList.CherryBlossoms, G_Flower.FlowerList.gentian, G_Flower.FlowerList.nemophila, G_Flower.FlowerList.hibiscus };
    private readonly List<G_Flower.FlowerList> attackFlowers = new List<G_Flower.FlowerList> { G_Flower.FlowerList.rose, G_Flower.FlowerList.sunflower, G_Flower.FlowerList.Calendula };

    [Serializable]
    private class TutorialStrings
    {
        public string animal, wreath, attackWreath0, attackWreath1 , changeAttckWreath, transformWreath, objective, globeAmaranth, calendura, rose, sunflower, gentian, hibiscus, nemophila;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        arrive = Arrive.arrive0;
        explaneAttckWreathPhase = 0;
        seasonClock.useClockHand = false;
        showedTransformWreathTutorial = false;
        explanedWreath = false;
        explanedAnimal = false;
        explanedFlowers = new Dictionary<G_Flower.FlowerList, bool>();

        foreach(G_Flower.FlowerList explaneFlower in explaneFlowers)
        {
            explanedFlowers.Add(explaneFlower, false);
        }

        attackWreathTutorialCount = 0;
        playerHp = player.GetComponent<F_HP>();
        tutorialManager = G_TutorialManager.instance;

        G_FlowerEnergyReceiver.instance.onGetEnergy.AddListener(OnGetEnergy);

        playerStartPositipon = player.position;

        for(int i=0;i<isExplaneAttacks.Length;i++)
        {
            isExplaneAttacks[i] = false;
        }

        textScripter = G_TextScripter.instance;

        yield return new WaitUntil(() => G_FlowerBloomSystem.isAnalysed);

        yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.objective));
        //yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.bloom));
        StartCoroutine(phase());
    }

    public void ChangeSeasonScript()
    {
        //StartCoroutine(ChangeSeasonText());
    }

    public void OnLandingIsland()
    {
        if (!explanedAnimal)
        {
            explanedAnimal = true;
            StartCoroutine(OnLanding());
        }
    }
    
    private IEnumerator OnLanding()
    {
        yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.animal));
        yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.attackWreath0));
    }

    private IEnumerator ChangeSeasonText()
    {
        if(arrive == Arrive.arrive1)
        {
            yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.attackWreath1));
            arrive = Arrive.arrive2;
        }
        else if (arrive == Arrive.arrive2)
        {
            
        }
        else
        {
            yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.attackWreath0));
            arrive = Arrive.arrive1;
        }

    }

    private IEnumerator phase()
    {
        //Go around the islands
        for(float f = 0f; f < 1f; f += Time.deltaTime / aroundTime)
        {
            seasonClock.RotateClockHand(f);
            yield return null;
        }

        textScripter.StopAllCoroutines();
        yield return StartCoroutine(textScripter.ShowConversationEnumrator(textScripter.GetScript().changeToWinter2, true));
        yield return StartCoroutine(G_FadeMachine.FadeIn(0.5f));
        player.position = playerStartPositipon;
        gimmicks.SetActive(false);
        yield return StartCoroutine(G_FadeMachine.FadeOut(0.5f));
        witchSummoner.Summon();
        witchHp = witchSummoner.witch.GetComponent<F_HP>();
        Debug.Log(witchHp);

        for (float f = 0f; f < 1f; f += Time.deltaTime / againstWitchTime)
        {
            if (witchHp.isKilled || playerHp.isKilled)
            {
                break;
            }
            seasonClock.RotateClockHand(f);
            yield return null;
        }

        yield return StartCoroutine(G_FadeMachine.FadeIn(0.5f));

        seasonManager.SetSeason(G_SeasonManager.Season.Spring);
        skyController.ChangeSeasonForce(0);

        if (witchHp.isKilled)
        {
            yield return StartCoroutine(textScripter.ShowConversationEnumrator(textScripter.GetScript().witch_killed, true));
        }
        else if(playerHp.isKilled)
        {
            yield return StartCoroutine(textScripter.ShowConversationEnumrator(textScripter.GetScript().witch_playerKilled, true));
        }
        else
        {
            yield return StartCoroutine(textScripter.ShowConversationEnumrator(textScripter.GetScript().witch_timeOver, true));
        }
        S_BGMer.bgmExPlayer.Stop();
        SceneManager.LoadScene(titleSceneName);
        
    }

    private bool showedTransformWreathTutorial;
    private int attackWreathTutorialCount;
    private bool explanedWreath;
    public void OnGetEnergy(G_Flower.FlowerList flower)
    {
        if (G_SeasonManager.season != G_SeasonManager.Season.Winter)
        {
            if (!showedTransformWreathTutorial)
            {
                ExplaneTransformWreath();
            }
            if (attackWreathTutorialCount <= 1)
            {
                ExplaneAttackWreath();
            }

            if (explaneFlowers.Contains(flower))
            {
                if (!explanedFlowers[flower])
                {
                    print(flower);
                    int flowerAmount = G_FlowerEnergyReceiver.instance.GetCorollaAmount(flower);
                    if (flowerAmount >= 1)
                    {
                        explanedFlowers[flower] = true;
                        string keyName;
                        switch (flower)
                        {
                            case G_Flower.FlowerList.Calendula:
                                {
                                    keyName = tutorialStrings.calendura;
                                    break;
                                }
                            case G_Flower.FlowerList.GlobeAmaranth:
                                {
                                    keyName = tutorialStrings.globeAmaranth;
                                    break;
                                }
                            case G_Flower.FlowerList.rose:
                                {
                                    keyName = tutorialStrings.rose;
                                    break;
                                }
                            case G_Flower.FlowerList.sunflower:
                                {
                                    keyName = tutorialStrings.sunflower;
                                    break;
                                }
                            case G_Flower.FlowerList.gentian:
                                {
                                    keyName = tutorialStrings.gentian;
                                    break;
                                }
                            case G_Flower.FlowerList.hibiscus:
                                {
                                    keyName = tutorialStrings.hibiscus;
                                    break;
                                }
                            case G_Flower.FlowerList.nemophila:
                                {
                                    keyName = tutorialStrings.nemophila;
                                    break;
                                }
                            default:
                                {
                                    keyName = "";
                                    break;
                                }
                        }
                        tutorialManager.Tutorial(keyName);
                    }
                }
            }
        }

    }
    private void ExplaneTransformWreath()
    {
        var reciever = G_FlowerEnergyReceiver.instance;

        var energies = reciever.flowerEnergies.Where(flower =>
        transformFlowers.Contains(flower.Key)).Select(value=>
        value.Value.HaveWreath()).Where(b=>b).ToArray();

        if (energies.Length != 0)
        {
            showedTransformWreathTutorial = true;
            StartCoroutine(ExplaneWreath(tutorialStrings.transformWreath));
        }
    }
    private void ExplaneAttackWreath()
    {
        var reciever = G_FlowerEnergyReceiver.instance;

        var energies = reciever.flowerEnergies.Where(flower =>
        attackFlowers.Contains(flower.Key)).Select(value =>
        value.Value.HaveWreath()).Where(b => b).ToArray();

        if (energies.Length > attackWreathTutorialCount)
        {
            switch (attackWreathTutorialCount)
            {
                case 0:
                    {
                        //StartCoroutine(ExplaneWreath(tutorialStrings.attackWreath0));
                        break;
                    }
                case 1:
                    {
                        StartCoroutine(ExplaneWreath(tutorialStrings.changeAttckWreath));
                        break;
                    }
            }

            attackWreathTutorialCount++;
        }
    }
    private IEnumerator ExplaneWreath(string tutorialKey)
    {
        if (!explanedWreath)
        {
            yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialStrings.wreath));
            explanedWreath = true;
        }
        yield return StartCoroutine(tutorialManager.TutorialEnumerator(tutorialKey));
    }



}
