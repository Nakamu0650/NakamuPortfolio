using CriWare;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
public class S_TreeSound : MonoBehaviour
{
    
    S_SoundManager S_Sound;
    public List<string> treeAisacName = new List<string>();
    public CriAtomExPlayer treeExPlayer;
    List<float> currentAdaptiveMusicParam = new List<float>();
    private CriWare.CriAtomExPlayback playback = new CriWare.CriAtomExPlayback(CriWare.CriAtomExPlayback.invalidId);
    private CriAtomEx3dSource ex3dSource;
    public static string treename = "tree";
    //CriAtomEx3dListener ex3dListener;
    [SerializeField] Transform listenerPos;
    [SerializeField] Transform cameraPos;
    float frame = 0;
    [SerializeField] bool isSummer;
    // Start is called before the first frame update
    void Start()
    {
        if(isSummer)
        {
            S_SEManager.PlayTreeSE2(transform);
            Debug.Log("tree2");
        }
        else
        {
            S_SEManager.PlayTreeSE1(transform);
        }
        /*
        S_Sound = S_SoundManager.Instance;
        treeExPlayer = new CriWare.CriAtomExPlayer();
        ex3dSource = new CriAtomEx3dSource();
        //ex3dListener = new CriAtomEx3dListener();
        S_Sound.SetListener(treeExPlayer);
        treeExPlayer.Set3dSource(ex3dSource);
        ex3dSource.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        //UpdateListenerPosition(cameraPos, listenerPos);
        //S_Sound.SetListenerTransform(listenerPos);
        CriWare.CriAtomExPlayback playback = new CriWare.CriAtomExPlayback(CriWare.CriAtomExPlayback.invalidId);
        //S_Sound.PlaySoundAisac(treename, treeSE.AcbAsset.Handle, treeSE.CueId, treeAisacName, transform, true);

        //StartCoroutine(S_Sound.ChangeAisac("tree", 0, 10.0f, treeAisacName[0], 0));
        //S_Sound.PlaySound("tree", treeSE.AcbAsset.Handle, treeSE.CueId, transform, true);
        Debug.Log("tree0");
        PlayTreeControl(treeSE.AcbAsset.Handle, treeSE.CueId, treeAisacName);
        
       // S_Sound.ChangeAisac("tree", 0, 2.0f, treeAisacName[0], 0);
        */
    }
    
    // Update is called once per frame
    void Update()
    {
        //frame++;
         
         //UpdateListenerPosition(cameraPos, listenerPos);
        //frame = 0;
       
        /*
        if (S_BGMer.changeSeason)
        {
            Debug.Log("change");
            if (G_SeasonManager.season == G_SeasonManager.Season.Winter)
            {
                treeExPlayer.Stop();
            }
            else if (G_SeasonManager.season == G_SeasonManager.Season.Summer)
            {
                Debug.Log("kitaa");
                //StartCoroutine(S_Sound.ChangeAisac("tree",1, 10.0f, treeAisacName[0], 0));
                //S_Sound.ChangeAisac("tree", 1, 10.0f, treeAisacName[0], 0);
                StartCoroutine(ChangeAisacParamWithLerp(1, 2.0f, treeAisacName[0],0));
            }
            else if (G_SeasonManager.season == G_SeasonManager.Season.Spring)
            {
                //S_Sound.PlaySoundAisac("tree", treeSE.AcbAsset.Handle, treeSE.CueId, treeAisacName, transform, true);
                //StartCoroutine(S_Sound.ChangeAisac("tree",0, 2.0f, treeAisacName[0], 0));
                //S_Sound.ChangeAisac("tree", 0, 2.0f, treeAisacName[0], 0);
                StartCoroutine(ChangeAisacParamWithLerp(0, 2.0f, treeAisacName[0], 0));
            }
            else
            {
                //StartCoroutine(S_Sound.ChangeAisac("tree",0, 2.0f, treeAisacName[0], 0));
                //S_Sound.ChangeAisac("tree", 0, 2.0f, treeAisacName[0], 0);
                StartCoroutine(ChangeAisacParamWithLerp(0, 2.0f, treeAisacName[0], 0));
            }
            S_BGMer.changeSeason = false;
        }
        //S_Sound.UpdateListenerPosition(cameraPos);
        */
    }

    public void ChangeSeasonBGM()
    {
        
        
    }

    /*public void UpdateListenerPosition(Transform CameraTransform,Transform player)
    {
          ex3dListener.SetOrientation(CameraTransform.rotation.eulerAngles.x, CameraTransform.rotation.eulerAngles.y, CameraTransform.rotation.eulerAngles.z, 0, 1, 0);
          ex3dListener.SetPosition(player.transform.position.x, player.transform.position.y, player.transform.position.z);
          ex3dListener.Update();

            //Debug.Log(_transform.position);
        
    }*/
    

    public void PlayTreeControl(CriAtomExAcb TreeAcb, int cueid, List<string> aisac,bool is3D=true)
    {
        Debug.Log("tree1");
        treeExPlayer.SetCue(TreeAcb,cueid);
        if (is3D)
        {
           
            ex3dSource.SetPosition(transform.position.x, transform.position.y, transform.position.z);
            ex3dSource.Update();
        }
        currentAdaptiveMusicParam.Clear();
        for (int i = 0; i < aisac.Count; i++)
        {
           
            currentAdaptiveMusicParam.Add(0);
            treeExPlayer.SetAisacControl(aisac[i], currentAdaptiveMusicParam[i]);
        }
        Debug.Log("tree2");
        treeExPlayer.SetFirstBlockIndex(0);
        Debug.Log("tree3");
        playback = treeExPlayer.Start();
        Debug.Log("tree4");

    }

    public IEnumerator ChangeAisacParamWithLerp(float value, float time, string aisac, int number)
    {
        float targetValue = Mathf.Clamp01(value);
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            treeExPlayer.SetAisacControl(aisac, Mathf.Lerp(currentAdaptiveMusicParam[number], targetValue, Mathf.Clamp01(t / time)));
            treeExPlayer.Update(playback);
            yield return null;
        }
        currentAdaptiveMusicParam[number] = targetValue;
        treeExPlayer.SetAisacControl(aisac, currentAdaptiveMusicParam[number]);
        treeExPlayer.Update(playback);
    }
}
