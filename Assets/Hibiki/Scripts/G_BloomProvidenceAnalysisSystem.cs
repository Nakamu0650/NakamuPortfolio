using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class G_BloomProvidenceAnalysisSystem : MonoBehaviour
{
    public static G_BloomProvidenceAnalysisSystem instance;

    public AnalysisData data;
    private AnalysisData.Time timeData;

    public List<AnalysisData> saveDatas;

    private float playTime;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeData = AnalysisData.Time.GetTime();
        playTime = 0f;
        data = new AnalysisData()
        {
            time = timeData.ToStringTime(),
            playTime = 0,
            difficulty = GameManager.instance.difficulty.ToString(),
            rose = 0,
            calendura = 0,
            cherryBlossoms = 0,
            nemophila = 0,
            hibiscus = 0,
            gentian = 0,
            globeAmaranth = 0,
            lilaKilled = false
        };

        saveDatas = CSVLoader.Load<AnalysisData>(GetPath());
    }

    private void Update()
    {
        playTime += Time.unscaledDeltaTime;
    }

    public void Save()
    {
        Debug.Log(GetPath());
        data.playTime = Mathf.RoundToInt(playTime);
        saveDatas.Add(data);
        CSVLoader.Save(GetPath(), saveDatas);
    }

    public string GetPath()
    {
        var time = AnalysisData.Time.GetTime();
        return Application.dataPath.Replace("_Data", "AnalysisData" + time.month.ToString() + time.day.ToString() + ".csv");
    }

    [Serializable]
    public class AnalysisData
    {
        public string time;
        public int playTime;
        public string difficulty;
        public int rose, sunflower, calendura, cherryBlossoms, nemophila, hibiscus, gentian, globeAmaranth;
        public bool lilaKilled;

        [Serializable]
        public class Time
        {
            public int year;
            public int month;
            public int day;
            public int hour;
            public int minute;

            public static Time GetTime()
            {
                var now = DateTime.Now;

                var time = new Time()
                {
                    year = now.Year,
                    month = now.Month,
                    day = now.Day,
                    hour = now.Hour,
                    minute = now.Minute
                };

                return time;
            }

            public string ToStringDay()
            {
                return ($"{year}年{month}月{day}日");
            }

            public string ToStringTime()
            {
                return ($"{hour}時{minute}分");
            }

            public override string ToString()
            {
                return (ToStringDay() + ToStringTime());
            }
        }

    }
}
