using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class G_FlowerGroup : MonoBehaviour
{
    [SerializeField] GameObject flowerEnergyPrefab;
    [SerializeField] float energyAcceration = 1f;
    [SerializeField] public int flowerLayer, bloomedFlowerLayer;
    [SerializeField, Range(0.01f, 1f)] public float energyScale = 0.05f;

    public static Dictionary<GameObject, G_FlowerGroup> flowerGroups;

    private List<G_Flower>[] flowers;
    private G_LOD lod;

    private Dictionary<G_Flower.FlowerList, FlowerEnergies> flowerEnergies;

    private bool[] isBloomed;

    private int seasonNumber;

    // Start is called before the first frame update
    void Start()
    {
        isBloomed = (new bool[4]).Select(b => false).ToArray();
        
        seasonNumber = G_SeasonManager.seasonNumber;
        gameObject.layer = flowerLayer;
    }

    int amount = 0;
    public void Bloom()
    {
        if (isBloomed[seasonNumber] || !G_FlowerBloomSystem.isAnalysed) { return; }
        lod.isRun = true;

        amount++;
        G_FlowerEnergyReceiver receiver = G_FlowerEnergyReceiver.instance;
        Transform player = receiver.transform;
        gameObject.layer = bloomedFlowerLayer;
        isBloomed[seasonNumber] = true;
        StartCoroutine(Grow());
        

        IEnumerator Grow()
        {
            List<G_Flower.FlowerList> flowerLists = new List<G_Flower.FlowerList>();

            for (int i= 0;i< flowers[seasonNumber].Count;i++)
            {
                var flower = flowers[seasonNumber][i];
                if (flowerEnergies[flower.flowerType].isBloomed) continue;

                Transform child = flower.transform;
                Vector3 size = child.localScale;
                child.localScale = Vector3.zero;
                child.gameObject.SetActive(true);
                S_SEManager.PlayFlowersBloomSE(transform);
                S_SEManager.PlayFlowersBloomSE2(transform);
                child.DOScale(size, 1f).SetEase(Ease.OutSine);

                FlowerEnergies flowerEnergy = flowerEnergies[flower.flowerType];

                if (flowerLists.Contains(flower.flowerType))
                {
                    yield return null;
                    continue;
                }
                else
                {
                    flowerLists.Add(flower.flowerType);
                }

                foreach(var energyData in flowerEnergy.energies)
                {
                    if (energyData.gameObject == null)
                    {
                        continue;
                    }
                    energyData.gameObject.SetActive(true);
                    StartCoroutine(CarryEnergy(energyData.gameObject.transform, flower.flowerType, energyData.amount));
                    yield return null;
                }

            }
        }


        

        IEnumerator CarryEnergy(Transform energy, G_Flower.FlowerList flowerList, int amount)
        {
            if (!energy)
            {
                yield break;   
            }

            float height = energy.position.y;
            energy.DOMoveY(height + 1f, 1f).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(1f);

            float speed = 0f;
            Vector3 direction = player.position - energy.position;
            while (Vector3.SqrMagnitude(direction) > 0.25f)
            {
                direction = (player.position - energy.position);
                speed += energyAcceration;

                energy.position += speed * direction.normalized * Time.deltaTime;

                yield return null;
            }

            receiver.GetEnergy(flowerList, amount);
            try
            {
                Destroy(energy.gameObject);
            }
            catch
            {
                Debug.Log(flowerList + gameObject.name,gameObject);
            }
        }
    }

    public void Wither()
    {

    }

    /// <summary>
    /// 季節が変わる際に、咲く花を変化させる
    /// </summary>
    public void OnChangeSeason()
    {
        seasonNumber = G_SeasonManager.seasonNumber;
        gameObject.layer = (isBloomed[seasonNumber])? bloomedFlowerLayer : flowerLayer;

        for(int i = 0; i < flowers.Length; i++)
        {
            bool active = (i == seasonNumber);
            bool isBloom = isBloomed[i];
            lod.isRun = isBloom;
            if (isBloom)
            {
                flowers[i].ForEach(flower => flower.transform.DOScale((active ? 1f : 0f), 1f));
            }
            else
            {
                flowers[i].ForEach(flower => flower.gameObject.SetActive(false));
            }
        }
    }

    /// <summary>
    /// BloomSystemから非同期で呼び出し、花を咲かせる準備をする
    /// </summary>
    public void AnalysStart(Transform parent)
    {
        AnalysFlowers(parent);
    }

    /// <summary>
    /// グループに含まれる花の数を計算する
    /// </summary>
    private void AnalysFlowers(Transform parent)
    {
        lod = GetComponent<G_LOD>();
        lod.isRun = false;
        flowerGroups.Add(gameObject, this);
        flowers = (new List<G_Flower>[4]).Select(list => new List<G_Flower>()).ToArray();
        flowerEnergies = new Dictionary<G_Flower.FlowerList, FlowerEnergies>();
        Dictionary<G_Flower.FlowerList, int> flowerEnergyScaleAmount = new Dictionary<G_Flower.FlowerList, int>();
        int energyScaleAmount = Mathf.RoundToInt(1f / energyScale);

        foreach(G_Flower.FlowerList list in Enum.GetValues(typeof(G_Flower.FlowerList)))
        {
            flowerEnergyScaleAmount.Add(list, 0);
            flowerEnergies.Add(list, new FlowerEnergies());
        }


        int iii = 0;
        foreach(Transform child in transform)
        {
            G_Flower flower = child.GetComponent<G_Flower>();
            if (!flower)
            {
                continue;
            }
            flower.LerpMaterial();
            foreach(G_SeasonManager.Season season in flower.applicableSeasons)
            {
                int seasonNumber = G_SeasonManager.SeasonToInt(season);
                flowers[seasonNumber].Add(flower);
            }
            var flowerType = flower.flowerType;
            if(flowerEnergyScaleAmount[flowerType] >= energyScaleAmount)
            {
                flowerEnergyScaleAmount[flowerType] = 0;

                var energy = Instantiate(flowerEnergyPrefab, child.position, Quaternion.identity);
                energy.transform.parent = parent;

                energy.name = "Energy" + iii;
                energy.SetActive(false);
                flowerEnergies[flowerType].energies.Add(new FlowerEnergies.Energy(energy, flowerEnergyScaleAmount[flowerType]));
            }
            flowerEnergyScaleAmount[flowerType] += 1;
            iii++;
        }
        foreach(var pair in flowerEnergyScaleAmount)
        {
            if (pair.Value != 0)
            {
                Vector2 random = UnityEngine.Random.insideUnitCircle;
                var energy = Instantiate(flowerEnergyPrefab, transform.position + new Vector3(random.x,0f,random.y), Quaternion.identity);
                energy.transform.parent = parent;
                energy.name = "EnergyLast";
                energy.SetActive(false);
                flowerEnergies[pair.Key].energies.Add(new FlowerEnergies.Energy(energy, flowerEnergyScaleAmount[pair.Key]));
            }
        }

    }

    private class FlowerEnergies
    {
        public bool isBloomed = false;
        public List<Energy> energies = new List<Energy>();

        public class Energy
        {
            public GameObject gameObject;
            public int amount;

            public Energy(GameObject _gameObject, int _amount)
            {
                gameObject = _gameObject;
                amount = _amount;
            }
        }
    }
}
