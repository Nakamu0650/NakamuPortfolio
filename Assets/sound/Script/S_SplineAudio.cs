using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class S_SplineAudio : MonoBehaviour
{
    [SerializeField] CriWare.Assets.CriAtomCueReference splineSound;
    [SerializeField] string audioKey;
    [SerializeField] Transform audioSource;
    [SerializeField] Transform listenerTransform;
    [SerializeField] Transform CameraTransrom;
    [Header("Reduce the burden")]
    [SerializeField, Range(0.01f, 1f)] float updateFrequency = 1f;
    [SerializeField, Range(1, 4)] int splineResolution = 1,splineIteration=1;
    private SplineContainer _spline;
    private S_SoundManager manager;
    private int updateAmount;
    private int amount;
    private Vector3 roundNearestPoint;
    // Start is called before the first frame update
    void Start()
    {
        _spline = GetComponent<SplineContainer>();
        manager = S_SoundManager.Instance;
        manager.PlaySound(audioKey, splineSound.AcbAsset.Handle, splineSound.CueId, audioSource, true,true);

        //updateAmount = Mathf.RoundToInt(1f / updateFrequency);
        updateAmount = 16;
        amount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (amount % updateAmount == 0)
        {
            amount = 0;
        }
        else
        {
            amount++;
            return;
        }

        //Get nearest position between audioListener and audioSource(on spline).
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);

        float distance = SplineUtility.GetNearestPoint(spline, listenerTransform.position,out var nearestPoint, out var t,splineResolution,splineIteration);

        Vector3 nearest = nearestPoint;
        Vector3 round = RoundVector(nearest);
        if (round!=roundNearestPoint)
        {
            audioSource.position = nearestPoint;
            roundNearestPoint = round;
            audioSource.position = nearest;
            manager.UpdateSoundPosition(audioKey);
            manager.UpdateListenerPosition(CameraTransrom);
        }


    }

    private Vector3 RoundVector(Vector3 origin)
    {
        return new Vector3(Mathf.Round(origin.x), Mathf.Round(origin.y), Mathf.Round(origin.z));
    }
}
