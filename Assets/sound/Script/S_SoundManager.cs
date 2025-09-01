using System;
using System.Collections.Generic;
using UnityEngine;

using CriWare;
//using static CriWare.CriProfiler;
using System.Collections;
using UnityEngine.InputSystem.Utilities;

public class S_SoundManager : IDisposable
{
    // Variables that manage the ExPlayer
    private Dictionary<string, MyExPlayer> _exPlayers;
    private CriAtomEx3dListener _ex3dListener;    // ExListener
    private Transform _transform;
   

    // ========================================================================================
    //Description to make ADXSoundManager a singleton

    private static S_SoundManager _instance;

    
    //ADXSoundManager can be accessed from anywhere with the description ADXSoundManager.Instance
    public static S_SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new S_SoundManager();
            }
            return _instance;
        }
    }

    // Make constructors private to prevent external instantiation
    private S_SoundManager()
    {
        _exPlayers = new Dictionary<string, MyExPlayer>();
        _ex3dListener = new CriAtomEx3dListener();
       

        // --------------------------------------------------------------------------------
        // If there is any initialization code required by ADXSoundManager, it can be written in this code block as appropriate.
        // 
        // int myFavoriteThings = 100;
        // myFavoriteThings += 10;
        // --------------------------------------------------------------------------------
    }
    // Singleton description of ADXSoundManager so far
    // ========================================================================================

    // ???????????i?O???j??????????
    public void DisposeDummyNativeListener()
    {
        CriAtomListener.DestroyDummyNativeListener();
    }

    // Destroying Resources
    public void Dispose()
    {
        // Discard all exPlayer
        foreach (var exPlayer in _exPlayers.Values)
        {
            exPlayer.Dispose();
        }
        _exPlayers.Clear();

        // Destroy Ex3dListener
        _ex3dListener.Dispose();

        GC.SuppressFinalize(this);
    }

    private MyExPlayer GetOrCreateExPlayer(string key)
    {
        
        if (!_exPlayers.ContainsKey(key))
        {
            // Create an ExPlayer with a name corresponding to the key if it does not already exist.
            _exPlayers[key] = new MyExPlayer();
        }

        return _exPlayers[key];
    }

    private MyExPlayer CreateExPlayer(string key)
    {
       
      
            // Create an ExPlayer with a name corresponding to the key if it does not already exist.
       _exPlayers[key] = new MyExPlayer();
        

        return _exPlayers[key];
    }

    public void PlaySound(string key, CriAtomExAcb cueSheet, string cueName, Transform transform, bool is3D,bool isLoop = false)
    {
        //   key = "BGMExPlayer",Bullet
        //PlaySound("BGMExPlayer",cueReference.acb,)
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        if (is3D)
        {
            exPlayer.SetTransform(transform);
        }
        exPlayer.Play(cueSheet, cueName, is3D);
    }

    public void Stop(string key)
    {
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        exPlayer.Stop();
    }

    public void PlaySound(string key, CriAtomExAcb cueSheet, int cueId, Transform transform, bool is3D,bool isLoop= false,bool isFade = false)
    {
        //Debug.Log("tree5.1");
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        //Debug.Log("tree5.2");
        if (is3D)
        {
          //  Debug.Log("tree5.3");
            exPlayer.SetTransform(transform);
            //Debug.Log("tree5.4");
        }
        //Debug.Log("tree6");
        exPlayer.Play(cueSheet, cueId, is3D);
    }

    public void PlaySoundAisac(string key, CriAtomExAcb cueSheet, int cueId, List<string> aisac,Transform transform, bool is3D, bool isLoop = false, bool isFade = false)
    {
        MyExPlayer exPlayer = CreateExPlayer(key);
        if (is3D)
        {
            exPlayer.SetTransform(transform);
        }
        exPlayer.PlayBGMControl(cueSheet, cueId, aisac, is3D);
    }

    public void ChangeAisac(string key, float value, float time, string aisac, int number)
    {
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        //Debug.Log("tree0");
        
        exPlayer.ChangeAisacParamWithLerp(value,time, aisac,number);
        //Debug.Log("tree5");
        //yield return null;
    }


    public void ChangeVolume(string categoryName, float volume)
    {
        CriWare.CriAtom.SetCategoryVolume(categoryName, volume);
    }



    // Function to update the position of ex3dSource associated with exPlayer
    
    public void UpdateSoundPosition(string key)
    {
       
        if (_exPlayers.TryGetValue(key, out MyExPlayer exPlayer))
        {
            
            exPlayer.Update();
           
        }
    }

    //Set Transforms to be used as listeners (camera, characters, etc.)
    public void SetListenerTransform(Transform lisenerTransform)
    {
        _transform = lisenerTransform;
        //Debug.Log(_transform.name);
    }

    public void SetListener(CriAtomExPlayer explayer)
    {
        explayer.Set3dListener(_ex3dListener);
    }

    // Call Update() of the associated GameObject to update this position.
    public void UpdateListenerPosition(Transform CameraTransform)
    {
        if (_transform != null)
        {
            
            _ex3dListener.SetOrientation(CameraTransform.rotation.eulerAngles.x, CameraTransform.rotation.eulerAngles.y, CameraTransform.rotation.eulerAngles.z, 0, 1, 0);
            _ex3dListener.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
            _ex3dListener.Update();
            
            //Debug.Log("listenerPos:"+_transform.position);
        }
    }

    public void SetSelecterLabelForAllExPlayer(string selectorName, string selectorLabelName)
    {
        foreach (MyExPlayer exPlayer in _exPlayers.Values)
        {
            exPlayer.SetSelectorLabel(selectorName, selectorLabelName);

        }
    }
    // ========================================================================================
    // Description of MyExPlayer class from here
    private class MyExPlayer
    {
        private CriAtomExPlayer _exPlayer;
        private CriAtomEx3dSource _ex3dSource;

        // Coordinates of the GameObject that will be the sound source
        private Transform _transform;
        List<float> currentAdaptiveMusicParam = new List<float>();
        CriAtomExPlayback playback = new CriWare.CriAtomExPlayback(CriAtomExPlayback.invalidId);

        // initialization process

        public void SetSelectorLabel(string selectorName, string selectorLacbName)
        {
            _exPlayer.SetSelectorLabel(selectorName, selectorLacbName);

        }
        public MyExPlayer()
        {
            _exPlayer = new CriAtomExPlayer();
            _ex3dSource = new CriAtomEx3dSource();
        }
        
        public void SetTransform(Transform transform)
        {
            _transform = transform;
            _exPlayer.Set3dSource(_ex3dSource);
        }

        public void Play(CriAtomExAcb cueSheet, string cueName, bool is3D)
        {
            _exPlayer.SetCue(cueSheet, cueName);

            if (is3D && _transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }
            

            // Play sound
            _exPlayer.Start();

            

        }

        public void Play(CriAtomExAcb cueSheet, int cueId, bool is3D)
        {
            _exPlayer.SetCue(cueSheet, cueId);
            //Debug.Log("tree7");
            if (is3D && _transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }
            /*if(isLoop)
            {
                _exPlayer.Loop(isLoop);
            }*/
            //Debug.Log("tree8");
            // Play sound
            _exPlayer.Start();
            //Debug.Log("tree9");
        }

        public void PlayBGMControl(CriAtomExAcb currentBGMAcb, int cueid, List<string> aisac,bool is3D)
        {
            _exPlayer.SetCue(currentBGMAcb, cueid);
            if (is3D && _transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }
            currentAdaptiveMusicParam.Clear();
            for (int i = 0; i < aisac.Count; i++)
            {
                currentAdaptiveMusicParam.Add(0);
                _exPlayer.SetAisacControl(aisac[i], currentAdaptiveMusicParam[i]);
            }
            _exPlayer.SetFirstBlockIndex(0);
            playback = _exPlayer.Start();

        }

        public void ChangeAisacParamWithLerp(float value, float time, string aisac, int number)
        {
            
            float targetValue = Mathf.Clamp01(value);
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
               
                _exPlayer.SetAisacControl(aisac, Mathf.Lerp(currentAdaptiveMusicParam[number], targetValue, Mathf.Clamp01(t / time)));
                _exPlayer.Update(playback);
                //yield return null;
            }
            currentAdaptiveMusicParam[number] = targetValue;
            _exPlayer.SetAisacControl(aisac, currentAdaptiveMusicParam[number]);
            _exPlayer.Update(playback);
        }

        public void Update()
        {
            if (_transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }
        }

        public void Stop()
        {
            _exPlayer.Stop();
        }

        public void Dispose()
        {
            _exPlayer.Dispose();
            _ex3dSource.Dispose();
        }

        public void Volume(float setVolume)
        {
            _ex3dSource.SetVolume(setVolume);
        }
    }
}
