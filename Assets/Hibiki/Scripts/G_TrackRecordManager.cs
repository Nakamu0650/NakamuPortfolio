using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class G_TrackRecordManager : MonoBehaviour
{
    public static G_TrackRecordManager instance;

    public List<G_TrackRecord> trackRecords = new List<G_TrackRecord>();
    [SerializeField] RectTransform trackRecordPanel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image recordImage;
    [SerializeField] float showSeconds = 3f;
    [SerializeField] float moveSeconds = 1f;
    private bool onDisplay;

    private AchiveSaveData saveData;
    private static string trackString = "";
    private float moveSize;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        saveData = DataRW.Load<AchiveSaveData>("Achives");

        onDisplay = false;
        trackString = "";
        trackRecordPanel.anchoredPosition = new Vector2(125f, trackRecordPanel.anchoredPosition.y);
        moveSize = trackRecordPanel.sizeDelta.x / 2f;
    }

    public bool AchiveTrackRecord(string _trackRecordString)
    {
        var record = trackRecords.Where(t => (_trackRecordString == t.trackName)).ToArray();
        if(record.Length != 0)
        {
            return AchiveTrackRecord(record[0]);
        }
        return false;
    }

    public bool AchiveTrackRecord(G_TrackRecord record)
    {
        if(!saveData.achives.TryAdd(record.trackName, true))
        {
            bool isGot = saveData.achives[record.trackName];
            if (isGot)
            {
                return false;
            }
        }
        saveData.achives[record.trackName] = true;
        DataRW.Save(saveData, "Achives");

        StartCoroutine(Achive(record));
        return true;
    }

    private IEnumerator Achive(G_TrackRecord record)
    {
        yield return new WaitUntil(() => !onDisplay);
        onDisplay = true;
        nameText.text = record.title.GetValue();
        if (record.sprite)
        {
            recordImage.sprite = record.sprite;
            Color c = recordImage.color;
            recordImage.color = new Color(c.r, c.g, c.b, 1f);
        }
        else
        {
            Color c = recordImage.color;
            recordImage.color = new Color(c.r, c.g, c.b, 0f);
        }

        trackRecordPanel.DOAnchorPosX(-moveSize, moveSeconds).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(moveSeconds + showSeconds);
        trackRecordPanel.DOAnchorPosX(moveSize, moveSeconds).SetEase(Ease.InSine);
        yield return new WaitForSeconds(moveSeconds);
        onDisplay = false;
    }

    [System.Serializable]
    public class AchiveSaveData
    {
        public SerializedDictionary<bool> achives = new SerializedDictionary<bool>();

        public AchiveSaveData()
        {
            achives = new SerializedDictionary<bool>();
        }
    }
}
