using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class G_SkyController : MonoBehaviour
{
    [SerializeField] G_SeasonManager seasonManager;
    [SerializeField] Light sun;
    [SerializeField] LensFlareComponentSRP lensFlare;
    [SerializeField] float minSunIntensity = 1f;
    [SerializeField] Color[] topColor = new Color[4];
    [SerializeField] Color[] bottomColor = new Color[4];
    [SerializeField] Color[] fogColor = new Color[4];
    [SerializeField] float[] cloudAmountFromSeason = new float[4];
    [SerializeField] float[] fogDensityFromSeason = new float[4];

#if UNITY_EDITOR
    [SerializeField, Range(0, 3)] int debugSeasonNum = 0;
#endif

    public static Vector2 windSpeed = new Vector2(0, 0);
    private Vector2 windPosition;
    private Vector2 windSpeedNow;
    [Range(0, 1)] [SerializeField] float timePercentage=0;
    [Range(1, 5)] [SerializeField] float cloudAmount = 1;
    private Material skybox;

#if UNITY_EDITOR
    private void OnValidate()
    {
        skybox = RenderSettings.skybox;
        skybox.SetColor("_TopColor", topColor[debugSeasonNum]);
        skybox.SetColor("_BottomColor", bottomColor[debugSeasonNum]);
        RenderSettings.fogColor = fogColor[debugSeasonNum];
        RenderSettings.fogDensity = fogDensityFromSeason[debugSeasonNum];
        cloudAmount = cloudAmountFromSeason[debugSeasonNum];
        SetSkyboxFromCloudAmount();
    }
#endif


    // Start is called before the first frame update
    void Start()
    {
        skybox = RenderSettings.skybox;
        windPosition = Vector2.zero;
        SetRandomWind();
        windSpeedNow = windSpeed;
        cloudAmount = cloudAmountFromSeason[0];
}

// Update is called once per frame
void Update()
    {
        skybox.SetFloat("_WindSpeedEast", windPosition.x);
        skybox.SetFloat("_WindSpeedNorth", windPosition.y);

        windSpeedNow = Vector2.Lerp(windSpeedNow, windSpeed, Time.deltaTime*0.1f);
        windPosition += windSpeedNow * Time.deltaTime;
        skybox.SetFloat("_CloudAmount", cloudAmount);
        //SetSkyboxFromCloudAmount();
    }
    public static void SetRandomWind()
    {
        windSpeed = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(1f, 3f);
    }

    public void OnSeasonChange()
    {
        SetRandomWind();
        StartCoroutine(SetSkyFromCloudAmountFromSeason(G_SeasonManager.seasonNumber));
    }

    public void ChangeSeasonForce(int _seasonNumber)
    {
        StopAllCoroutines();
        skybox = RenderSettings.skybox;
        skybox.SetColor("_TopColor", topColor[_seasonNumber]);
        skybox.SetColor("_BottomColor", bottomColor[_seasonNumber]);
        RenderSettings.fogColor = fogColor[_seasonNumber];
        RenderSettings.fogDensity = fogDensityFromSeason[_seasonNumber];
        cloudAmount = cloudAmountFromSeason[_seasonNumber];
        SetSkyboxFromCloudAmount();
    }

    public void ChangeSeason(int _seasonNumber)
    {
        StopAllCoroutines();
        StartCoroutine(SetSkyFromCloudAmountFromSeason(_seasonNumber));
    }

    private void SetSkyboxFromCloudAmount()
    {
        float percent = Mathf.InverseLerp( cloudAmountFromSeason.Min(), cloudAmountFromSeason.Max(), cloudAmount);
        skybox.SetFloat("_CloudAmount", cloudAmount);
        skybox.SetFloat("_SkyBlightness", Mathf.Lerp(1f, 3f, percent));
        sun.intensity = Mathf.Max(Mathf.Lerp(2f, 0f, percent),minSunIntensity);
        lensFlare.intensity= Mathf.Lerp(1f, 0f, percent);
    }
    private IEnumerator SetSkyFromCloudAmountFromSeason(int seasonNumber)
    {
        int beforeSeasonNumber = (seasonNumber == 0) ? 3 : seasonNumber - 1;
        for(float f = 0; f < 1f; f += Time.deltaTime*0.25f)
        {
            skybox.SetColor("_TopColor", Color.Lerp( topColor[beforeSeasonNumber], topColor[seasonNumber], f));
            skybox.SetColor("_BottomColor", Color.Lerp(bottomColor[beforeSeasonNumber], bottomColor[seasonNumber], f));
            RenderSettings.fogColor = Color.Lerp(fogColor[beforeSeasonNumber], fogColor[seasonNumber], f);

            cloudAmount = Mathf.Lerp(cloudAmountFromSeason[beforeSeasonNumber], cloudAmountFromSeason[seasonNumber], f);
            RenderSettings.fogDensity = Mathf.Lerp(fogDensityFromSeason[beforeSeasonNumber], fogDensityFromSeason[seasonNumber], f);
            SetSkyboxFromCloudAmount();
            yield return null;
        }
        cloudAmount = cloudAmountFromSeason[seasonNumber];
        RenderSettings.fogDensity = fogDensityFromSeason[seasonNumber];
        SetSkyboxFromCloudAmount();
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        skybox = RenderSettings.skybox;
        skybox.SetColor("_TopColor", topColor[debugSeasonNum]);
        skybox.SetColor("_BottomColor", bottomColor[debugSeasonNum]);
        RenderSettings.fogColor = fogColor[debugSeasonNum];
        RenderSettings.fogDensity = fogDensityFromSeason[debugSeasonNum];
        cloudAmount = cloudAmountFromSeason[debugSeasonNum];
        SetSkyboxFromCloudAmount();
#endif
    }
}
