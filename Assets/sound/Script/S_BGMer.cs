using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_BGMer : MonoBehaviour
{
    S_SoundManager S_Sound;
    [SerializeField] CriWare.Assets.CriAtomCueReference menuBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference fieldBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference tutrialBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference battleBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference BossBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference WolfBossBgm;
    [SerializeField] CriWare.Assets.CriAtomCueReference PVBGM;
    [SerializeField] CriWare.Assets.CriAtomCueReference environmentSE;
    CriWare.CriAtomExPlayback playback = new CriWare.CriAtomExPlayback(CriWare.CriAtomExPlayback.invalidId);
    public string bgmCategoryName;
    public string seCategoryName;
    public string voiseCategoryName;
    float generalPreviousVolume;
    float bgmPreviousVolume;
    float sePreviousVolume;
    float voisePreviousVolume;
    [Range(0, 1.0f)] public static float generalvalue;
    [Range(0, 1.0f)] public static float bgmvalue;
    [Range(0, 1.0f)] public static float sevalue;
    [Range(0, 1.0f)] public static float voisevalue;
    bool isNext = false;
    //[SerializeField] bool isTitle = false;
    [SerializeField] bool isMenu = false;
    [SerializeField] bool isBoss = false;
    [SerializeField] bool isField = false;
    [SerializeField] bool isTutrial = false;
    [SerializeField] bool isEnvironment = false;
    [SerializeField] bool isWolfBoss = false;
    [SerializeField] bool isPV = false;
    public static CriWare.CriAtomExPlayer bgmExPlayer;
    bool isChange = false;
    public List<string> fieldAisacName = new List<string>();
    public List<string> tutrialAisacName = new List<string>();
    public List<string> bossAisacName = new List<string>();
    public const string fieldMusicName = "fieldMusic";
    public const string bossMusicName = "bossMusic";
    public static string titlebgmName;
    List<float> currentAdaptiveMusicParam = new List<float>();
    [SerializeField]bool isWinterChange = true;
    P_CameraMove_Alpha cameramove;
    public static bool changeSeason = false;
    bool isFly;
    bool Area1;
    int blockid;
    float lilatime = 0;
    bool lilaStart;
    // Start is called before the first frame update
    void Start()
    {
        blockid = 1;
        isFly = false;
        Area1 = false;
        S_Sound = S_SoundManager.Instance;
        bgmExPlayer = new CriWare.CriAtomExPlayer();
        bgmPreviousVolume = bgmvalue;
        sePreviousVolume = sevalue;
        voisePreviousVolume = voisevalue;
        generalPreviousVolume = generalvalue;
        bgmExPlayer.Stop();
        changeSeason = false;
        lilaStart = false;
        lilatime = 0;
        if (isMenu)
        {
            titlebgmName = "menubgm";
            S_Sound.PlaySound(titlebgmName, menuBgm.AcbAsset.Handle, menuBgm.CueId, transform, false);
        }
        if (isField)
        {
            PlayBGMControl(fieldBgm.AcbAsset.Handle, fieldBgm.CueId,fieldAisacName);
            StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, fieldAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[1], 1));
            StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[2], 2));
            StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[2], 3));
        }
        if (isTutrial)
        {
            PlayBGMControl(tutrialBgm.AcbAsset.Handle, tutrialBgm.CueId, tutrialAisacName);
            StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, tutrialAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, tutrialAisacName[1], 1));
        }
        if (isBoss)
        {
            Debug.Log("invalidid:" + CriWare.CriAtomExPlayback.invalidId);
            PlayBGMControl(BossBgm.AcbAsset.Handle, BossBgm.CueId,bossAisacName);
            isBoss = false;
        }
        if (isEnvironment)
        {
            S_Sound.PlaySound("environment", environmentSE.AcbAsset.Handle, environmentSE.CueId, transform, false);
        }
        if(isPV)
        {
            titlebgmName = "PV";
            S_Sound.PlaySound(titlebgmName, PVBGM.AcbAsset.Handle,PVBGM.CueId, transform, false);
        }
        if (isWolfBoss)
        {
            S_Sound.PlaySound("wolfbossBGM", WolfBossBgm.AcbAsset.Handle, WolfBossBgm.CueId, transform, false);
        }
        CriWare.CriAtom.SetCategoryVolume(bgmCategoryName, generalvalue * bgmvalue) ;
        CriWare.CriAtom.SetCategoryVolume(seCategoryName, generalvalue * sevalue);
        cameramove = GameObject.Find("MainFreeLookCamera").GetComponent<P_CameraMove_Alpha>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lilaStart)
        {
            lilatime += Time.deltaTime;
        }
        if(cameramove != null)
        {

            if(cameramove.ExistAngryEnemy()&&!isBoss)
            {
                if (!isChange)
                {
                    
                    
                    
                    Debug.Log("isEnemy");
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[0], 0));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[1], 1));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[2], 2));
                    StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, fieldAisacName[3], 3));
                    isChange = true;
                }
                
            }
            else if (isChange)
            {
                if (G_SeasonManager.season == G_SeasonManager.Season.Spring)
                {

                    //Debug.Log("S_field1");
                    //bgmExPlayer.Stop();
                    //PlayBGMControl(fieldBgm.AcbAsset.Handle, fieldBgm.CueId, fieldAisacName);
                    //Debug.Log("S_field2");

                    //Debug.Log("S_haru");
                    StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, fieldAisacName[0], 0));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[1], 1));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[2], 2));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[3], 3));
                }
                else if (G_SeasonManager.season == G_SeasonManager.Season.Summer)
                {
                    Debug.Log("S_natu");
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[0], 0));
                    StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, fieldAisacName[1], 1));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[2], 2));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[3], 3));
                }
                else if (G_SeasonManager.season == G_SeasonManager.Season.Autumn)
                {
                    Debug.Log("S_aki");
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[0], 0));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[1], 1));
                    StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, fieldAisacName[2], 2));
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, fieldAisacName[3], 3));
                }

                /*if (isTutrial)
                {
                    StartCoroutine(ChangeAisacParamWithLerp(0, 0.5f, tutrialAisacName[0], 0));
                    StartCoroutine(ChangeAisacParamWithLerp(1, 0.5f, tutrialAisacName[1], 1));
                }*/
                isChange = false;
            }
        }
        /* Get BusAnalyzerInfo from a DSP Buss verifyed by mDspBusId*/
        if (bgmvalue != bgmPreviousVolume)
        {
            S_Sound.ChangeVolume(bgmCategoryName, generalvalue * bgmvalue);
            bgmPreviousVolume = bgmvalue;
        }
        if (sevalue != sePreviousVolume)
        {
            S_Sound.ChangeVolume(seCategoryName, generalvalue * sevalue);
            sePreviousVolume = sevalue;
        }
        if(generalvalue != generalPreviousVolume)
        {
            generalPreviousVolume = generalvalue;
            S_Sound.ChangeVolume(bgmCategoryName, generalvalue * bgmvalue);
            S_Sound.ChangeVolume(seCategoryName, generalvalue * sevalue);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isNext)
            {
                Debug.Log("a1");
                SetBlockId(2);
                isNext = true;
            }
            else
            {
                SetBlockId(3);
            }
        }
        

        if(G_SeasonManager.isChangedSeason)
        {
            
        }

    }

    public void Area1BGM()
    {
        if (!Area1)
        {
            
            StartCoroutine(ChangeAisacParamWithLerp(1, 5.0f, tutrialAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(0, 5.0f, tutrialAisacName[1], 1));
            Debug.Log("area1");
            Area1 = true;
        }
    }

    public void Area1KillBGM()
    {
        StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, tutrialAisacName[0], 0));
        StartCoroutine(ChangeAisacParamWithLerp(1, 5.0f, tutrialAisacName[1], 1));
        Debug.Log("killed");
    }

    public void Area1ExitBGM()
    {
        if (isFly == false)
        {
            /*StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, tutrialAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, tutrialAisacName[1], 1));
            */
            //bgmExPlayer.SetFadeOutTime(3);
            isFly = true;
        }
    }

    public void Area2BGM()
    {
        Debug.Log("haru");
        bgmExPlayer.Stop();
        PlayBGMControl(fieldBgm.AcbAsset.Handle, fieldBgm.CueId, fieldAisacName);
        Debug.Log("haru2");
        StartCoroutine(ChangeAisacParamWithLerp(1, 3.0f, fieldAisacName[0], 0));
        StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[1], 1));
        StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[2], 2));
        StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[2], 3));
        Debug.Log("haru3");
    }

    public void LilaKillEary()
    {
        
        bgmExPlayer.Stop();
    }
    public void ChangeSeasonBGM()
    {
        changeSeason = true;
        Debug.Log("changeseason:" + changeSeason);
        if (G_SeasonManager.season == G_SeasonManager.Season.Spring)
        {
            isBoss = false;
            lilaStart = false;
            //bgmExPlayer.Stop();
            //PlayBGMControl(fieldBgm.AcbAsset.Handle, fieldBgm.CueId, fieldAisacName);


            //S_Sound.PlaySound("environment", environmentSE.AcbAsset.Handle, environmentSE.CueId, transform, false);
            StartCoroutine(ChangeAisacParamWithLerp(1, 2.0f, fieldAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(0, 2.0f, fieldAisacName[1], 1));
            StartCoroutine(ChangeAisacParamWithLerp(0, 2.0f, fieldAisacName[2], 2));
        }
        else if (G_SeasonManager.season == G_SeasonManager.Season.Summer)
        {
            isBoss = false;
            
            StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(1, 3.0f, fieldAisacName[1], 1));
            StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[2], 2));
        }
        else if (G_SeasonManager.season == G_SeasonManager.Season.Autumn)
        {
            isBoss = false;
           
            StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[0], 0));
            StartCoroutine(ChangeAisacParamWithLerp(0, 3.0f, fieldAisacName[1], 1));
            StartCoroutine(ChangeAisacParamWithLerp(1, 3.0f, fieldAisacName[2], 2));
        }
        else
        {
            
            bgmExPlayer.Stop();
            isBoss = true;
            lilaStart = true;
            PlayBGMControl(BossBgm.AcbAsset.Handle, BossBgm.CueId, bossAisacName);
            S_Sound.Stop("environment");
        }
    }

    public void PlayBGMControl(CriWare.CriAtomExAcb currentBGMAcb,int cueid, List<string> aisac,bool isFade=true)
    {
        bgmExPlayer.SetCue(currentBGMAcb,cueid);
        currentAdaptiveMusicParam.Clear();
        for (int i = 0; i < aisac.Count; i++)
        {
            currentAdaptiveMusicParam.Add(0);
            bgmExPlayer.SetAisacControl(aisac[i], currentAdaptiveMusicParam[i]);
        }
        /*bgmExPlayer.SetFirstBlockIndex(0);
        if (isFade)
        {
            bgmExPlayer.AttachFader();
            bgmExPlayer.SetFadeInTime(20);
            bgmExPlayer.SetFadeOutTime(20);
        }*/
        playback = bgmExPlayer.Start();

    }

    public void ChangeWinter()
    {
        /*
        Debug.Log("changewinter");
        bgmExPlayer.Stop();
        isBoss = true;
        PlayBGMControl(BossBgm.AcbAsset.Handle, BossBgm.CueId, bossAisacName);
        S_Sound.Stop("environment");
        */
    }

    public void StopBGMControl()
    {
       bgmExPlayer.Stop();

    }

    public void NextBlock()
    {
        blockid++;
        Debug.Log("blockid" + blockid);
        if(lilatime <= 30&&blockid==3)
        {
            lilatime = 0;
            bgmExPlayer.Stop();
        }
        SetBlockId(blockid);
    }

    public IEnumerator ChangeAisacParamWithLerp(float value,float time,string aisac,int number)
    {
        float targetValue = Mathf.Clamp01(value);
        for(float t = 0f;t<time;t+= Time.deltaTime)
        {
            bgmExPlayer.SetAisacControl(aisac, Mathf.Lerp(currentAdaptiveMusicParam[number], targetValue, Mathf.Clamp01(t / time)));
            bgmExPlayer.Update(playback);
            yield return null;
        }
        currentAdaptiveMusicParam[number] = targetValue;
        bgmExPlayer.SetAisacControl(aisac, currentAdaptiveMusicParam[number]);
        bgmExPlayer.Update(playback);
    }

    public void SetBlockId(int id)
    {
        if (playback.id != CriWare.CriAtomExPlayback.invalidId)
        {
           
            if (playback.GetCurrentBlockIndex() != id)
            {
                
                playback.SetNextBlockIndex(id);
            }
        }
    }
}
