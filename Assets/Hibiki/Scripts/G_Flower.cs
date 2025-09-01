using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Flower : MonoBehaviour
{
    public enum FlowerList
    {
        None = -2,CherryBlossoms = -1,nemophila = 0, rose = 1, LilyOfTheValley = 2,
        hibiscus = 3, sunflower = 4, lily = 5,
        gentian = 6, clusterAmaryllis = 7, Calendula = 8, GlobeAmaranth = 9
    }
    [SerializeField] public FlowerList flowerType;
    [SerializeField] public List<G_SeasonManager.Season> applicableSeasons;
    [SerializeField] public float spreadDistance = 10f;
    [SerializeField] public List<Material> materials;
    [SerializeField] public bool isVerticalToGround = true;

    public void LerpMaterial()
    {
        var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        Material[] _materials = meshRenderer.materials;
        _materials[0] = materials[Random.Range(0, materials.Count)];
        transform.GetChild(0).GetComponent<MeshRenderer>().materials = _materials;
    }
}
