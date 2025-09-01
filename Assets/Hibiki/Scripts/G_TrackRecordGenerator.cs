using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G_TrackRecordGenerator : MonoBehaviour
{
    public enum Mode {None, Name, Object};
    public Mode mode;
    public G_TrackRecord record;
    public string trackName;
    public UnityEvent onAchived = new UnityEvent();

    private bool isGot;

    private void Start()
    {
        isGot = false;
    }

    public void Achive()
    {
        if (isGot) return;
        isGot = true;

        bool b = false;
        switch (mode)
        {
            case Mode.Name:
                {
                    b = G_TrackRecordManager.instance.AchiveTrackRecord(trackName);
                    break;
                }
            case Mode.Object:
                {
                    b = G_TrackRecordManager.instance.AchiveTrackRecord(record);
                    break;
                }
        }

        if (b)
        {
            onAchived.Invoke();
        }
    }
}
