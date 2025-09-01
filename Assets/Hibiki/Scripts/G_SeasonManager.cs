using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class G_SeasonManager : MonoBehaviour
{
    private Terrain terrain;
    [HideInInspector]public static Season season;
    [HideInInspector]public static int seasonNumber;
    [HideInInspector] public static float seasonTime;
    [HideInInspector] public static bool isChangedSeason;
    [HideInInspector] public static int numberOfYears;
    public static G_SeasonManager instance;
    public static float seasonTimeScale;
    [HideInInspector]
    public enum Season
    {
        Spring, Summer, Autumn, Winter
    }
    [Header("季節ごとの草の色を選択")]
    [SerializeField] Color[] grassColors = new Color[4];
    [SerializeField] Color[] grassDetailHealthyColors = new Color[4];
    [SerializeField] Color[] grassDetailDryColors = new Color[4];
    [SerializeField] Vector2 minSize, maxSize;
    [SerializeField] Vector2[] grassSizes = new Vector2[4];
    [SerializeField,Range(0f,1f)] float grassDetailGradientCache = 0.0333f;
    [SerializeField] AnimationCurve changeGrassCurve;

    [Header("草が含まれるレイヤーの番号を入力")]
    [SerializeField] List<int> grassLayerNumbers = new List<int>();

    [Header("季節のグラデーション速度")]
    [SerializeField] float changeSeasonSecond = 1f;

    [Header("季節ごとの長さを変更")]
    [SerializeField]public float[] seasonLengthes = new float[4] { 45f, 45f, 45f, 90f };

    [Header("季節ごとに呼ばれるイベント")]
    [SerializeField] UnityEvent seasonEvents = new UnityEvent();

    private void OnValidate()
    {
        terrain = GetComponent<Terrain>();
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        season = Season.Spring;
        terrain = GetComponent<Terrain>();
        seasonTimeScale = 1f;

        seasonTime = 0f;
        seasonNumber = 0;
        numberOfYears = 0;

        var terrainLayers = terrain.terrainData.terrainLayers;
        DetailPrototype[] grassDetails = terrain.terrainData.detailPrototypes;
        foreach (int layerNum in grassLayerNumbers)
        {
            terrainLayers[layerNum].diffuseRemapMax = grassColors[0];
        }
        Vector2 grassSize = grassSizes[seasonNumber];
        Vector2 newMinSize = Vector2.Scale(grassSize, minSize);
        Vector2 newMaxSize = Vector2.Scale(grassSize, maxSize);
        grassDetails[0].dryColor = grassDetailDryColors[0];
        grassDetails[0].healthyColor = grassDetailHealthyColors[0];
        grassDetails[0].minWidth = newMinSize.x;
        grassDetails[0].minHeight = newMinSize.y;
        grassDetails[0].maxWidth = newMaxSize.x;
        grassDetails[0].maxHeight = newMaxSize.y;
        terrain.terrainData.detailPrototypes = grassDetails;

    }

    // Update is called once per frame
    void Update()
    {
        if (isChangedSeason) isChangedSeason = false;
        if (seasonTime >= seasonLengthes[seasonNumber])
        {
            seasonTime = 0f;
            int beforeSeasonNum = seasonNumber;
            season = NextSeason(season);
            seasonNumber = SeasonToInt(season);
            StartCoroutine(ChangeGrassColor(beforeSeasonNum,seasonNumber));
            isChangedSeason = true;
            seasonEvents.Invoke();
        }
        seasonTime += Time.deltaTime*seasonTimeScale;

    }
    public void GoNextSeason()
    {
        seasonTime = 0f;
        int beforeSeasonNum = seasonNumber;
        season = NextSeason(season);
        seasonNumber = SeasonToInt(season);
        if (season == Season.Spring) { numberOfYears++; }
        StartCoroutine(ChangeGrassColor(beforeSeasonNum, seasonNumber));
        isChangedSeason = true;
        seasonEvents.Invoke();
    }
    public void SetSeason(Season changingSeason,bool callEvents=false)
    {
        StopAllCoroutines();
        seasonTime = 0f;
        season = changingSeason;
        if (season == Season.Spring) { numberOfYears++; }
        int _seasonNum = SeasonToInt(changingSeason);
        seasonNumber = _seasonNum;
        isChangedSeason = true;
        if (callEvents) { seasonEvents.Invoke(); }

        var terrainLayers = terrain.terrainData.terrainLayers;
        DetailPrototype[] grassDetails = terrain.terrainData.detailPrototypes;

        Color afterColorGrass = grassColors[_seasonNum];
        Color afterColorDry = grassDetailDryColors[_seasonNum];
        Color afterColorHealthy = grassDetailHealthyColors[_seasonNum];
        Vector2 afterGrassSize = grassSizes[_seasonNum];
        foreach (int layerNum in grassLayerNumbers)
        {
            terrainLayers[layerNum].diffuseRemapMax = afterColorGrass;
        }
        Vector2 newMinSize = Vector2.Scale(afterGrassSize, minSize);
        Vector2 newMaxSize = Vector2.Scale(afterGrassSize, maxSize);

        grassDetails[0].dryColor = afterColorDry;
        grassDetails[0].healthyColor = afterColorHealthy;
        grassDetails[0].minWidth = newMinSize.x;
        grassDetails[0].minHeight = newMinSize.y;
        grassDetails[0].maxWidth = newMaxSize.x;
        grassDetails[0].maxHeight = newMaxSize.y;
        terrain.terrainData.detailPrototypes = grassDetails;
    }

    public void SetSeasonSeemless(Season changingSeason, bool callEvents = false)
    {
        StopAllCoroutines();
        seasonTime = 0f;
        int beforeSeasonNum = SeasonToInt(season);
        season = changingSeason;
        if (season == Season.Spring) { numberOfYears++; }
        int _seasonNum = SeasonToInt(changingSeason);
        seasonNumber = _seasonNum;
        isChangedSeason = true;
        if (callEvents) { seasonEvents.Invoke(); }

        StartCoroutine(ChangeGrassColor(beforeSeasonNum, seasonNumber));
    }

    private IEnumerator ChangeGrassColor(int _beforeSeasonNum, int _afterSeasonNum)
    {
        yield return null;
        //Change grass color
        var terrainLayers = terrain.terrainData.terrainLayers;
        DetailPrototype[] grassDetails = terrain.terrainData.detailPrototypes;
        Color beforeColorGrass = grassColors[_beforeSeasonNum];
        Color afterColorGrass = grassColors[_afterSeasonNum];
        Color beforeColorDry = grassDetailDryColors[_beforeSeasonNum];
        Color afterColorDry = grassDetailDryColors[_afterSeasonNum];
        Color beforeColorHealthy = grassDetailHealthyColors[_beforeSeasonNum];
        Color afterColorHealthy = grassDetailHealthyColors[_afterSeasonNum];
        Vector2 beforeGrassSize = grassSizes[_beforeSeasonNum];
        Vector2 afterGrassSize = grassSizes[_afterSeasonNum];

        int i = 0;
        int cache = Mathf.RoundToInt(1f / grassDetailGradientCache);
        for(float f = 0f; f < 1f; f += Time.fixedDeltaTime/changeSeasonSecond)
        {
            Color nowColorGrass = (1 - f) * beforeColorGrass + f * afterColorGrass;
            foreach (int layerNum in grassLayerNumbers)
            {
                terrainLayers[layerNum].diffuseRemapMax = nowColorGrass;
            }

            if (i % cache == 0)
            {
                float grassDetail = changeGrassCurve.Evaluate(f);
                Color nowColorDry = (1 - grassDetail) * beforeColorDry + grassDetail * afterColorDry;
                Color nowColorHealthy = (1 - grassDetail) * beforeColorHealthy + grassDetail * afterColorHealthy;
                Vector2 nowMinSize = Vector2.Scale((1 - grassDetail) * beforeGrassSize + grassDetail * afterGrassSize,minSize);
                Vector2 nowMaxSize = Vector2.Scale((1 - grassDetail) * beforeGrassSize + grassDetail * afterGrassSize, maxSize);
                grassDetails[0].dryColor = nowColorDry;
                grassDetails[0].healthyColor = nowColorHealthy;
                grassDetails[0].minWidth = nowMinSize.x;
                grassDetails[0].minHeight = nowMinSize.y;
                grassDetails[0].maxWidth = nowMaxSize.x;
                grassDetails[0].maxHeight = nowMaxSize.y;
                terrain.terrainData.detailPrototypes = grassDetails;
            }
            i++;

            yield return new WaitForFixedUpdate();
        }
        foreach (int layerNum in grassLayerNumbers)
        {
            terrainLayers[layerNum].diffuseRemapMax = afterColorGrass;
        }
        Vector2 newMinSize = Vector2.Scale(afterGrassSize, minSize);
        Vector2 newMaxSize = Vector2.Scale(afterGrassSize, maxSize);

        grassDetails[0].dryColor = afterColorDry;
        grassDetails[0].healthyColor = afterColorHealthy;
        grassDetails[0].minWidth = newMinSize.x;
        grassDetails[0].minHeight = newMinSize.y;
        grassDetails[0].maxWidth = newMaxSize.x;
        grassDetails[0].maxHeight = newMaxSize.y;
        terrain.terrainData.detailPrototypes = grassDetails;
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        ResetTerrain();
    }
#endif
    private void ResetTerrain()
    {
        season = Season.Spring;
        terrain = GetComponent<Terrain>();
        seasonTime = 0f;
        seasonNumber = 0;

        var terrainLayers = terrain.terrainData.terrainLayers;
        DetailPrototype[] grassDetails = terrain.terrainData.detailPrototypes;
        foreach (int layerNum in grassLayerNumbers)
        {
            terrainLayers[layerNum].diffuseRemapMax = grassColors[0];
        }
        grassDetails[0].dryColor = grassDetailDryColors[0];
        grassDetails[0].healthyColor = grassDetailHealthyColors[0];
        grassDetails[0].minWidth = minSize.x;
        grassDetails[0].minHeight = minSize.y;
        grassDetails[0].maxWidth = maxSize.x;
        grassDetails[0].maxHeight = maxSize.y;
        terrain.terrainData.detailPrototypes = grassDetails;
    }


    public static int SeasonToInt(Season _season)
    {
        switch (_season)
        {
            case Season.Spring:
                return 0;
            case Season.Summer:
                return 1;
            case Season.Autumn:
                return 2;
            default:
                return 3;
        }
    }
    public static Season IntToSeason(int _num)
    {
        switch (_num)
        {
            case 0:
                return Season.Spring;
            case 1:
                return Season.Summer;
            case 2:
                return Season.Autumn;
            case 3:
                return Season.Winter;
            default:
                throw new System.Exception("Season not match Error.\nWhen converting int to Season, the number must be 0 to 3.");
        }
    }
    private static Season NextSeason(Season _season)
    {
        switch (_season)
        {
            case Season.Spring:
                return Season.Summer;
            case Season.Summer:
                return Season.Autumn;
            case Season.Autumn:
                return Season.Winter;
            default:
                return Season.Spring;
        }
    }

}
