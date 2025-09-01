using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class G_FlowerBloomSystem : MonoBehaviour
{
    [HideInInspector]public List<G_FlowerGroup> flowerGroups;
    public static bool isAnalysed = false;

    [SerializeField] LayerMask rayHitLayer;
    [SerializeField] int analysisSpeed = 10;

    private void Awake()
    {
        isAnalysed = false;
    }

    private void Start()
    {
        StartCoroutine(AnalysisFlowers(analysisSpeed));
    }

    public void OnChangeSeason()
    {
        foreach(G_FlowerGroup flower in flowerGroups)
        {
            flower.OnChangeSeason();
        }
    }

    private IEnumerator AnalysisFlowers(int _analysisSpeed)
    {
        var energyParent = new GameObject("Energies").transform;
        float timeScale = Time.timeScale;
        yield return null;
        Time.timeScale = 0f;
        var load = G_SceneManager.instance;
        G_FlowerGroup.flowerGroups = new Dictionary<GameObject, G_FlowerGroup>();
        flowerGroups = new List<G_FlowerGroup>();
        for(int i=0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            G_FlowerGroup flowerGroup = child.GetComponent<G_FlowerGroup>();
            G_LOD lod = child.GetComponent<G_LOD>();

            if (flowerGroup)
            {
                flowerGroups.Add(flowerGroup);

                flowerGroup.AnalysStart(energyParent);
            }
            if (lod)
            {
                lod.hitLayer = rayHitLayer;
            }

            if(i % _analysisSpeed == 0)
            {
                load.loadingValue = ((float)i / (float)transform.childCount);
                yield return null;
                //Debug.Log(((float)i / (float)transform.childCount).ToString("P"));
            }
        }
        load.loadingValue = 1f;
        Time.timeScale = timeScale;
        isAnalysed = true;
    }
}
