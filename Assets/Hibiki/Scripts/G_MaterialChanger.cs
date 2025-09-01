using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class G_MaterialChanger : MonoBehaviour
{
    [SerializeField] Ease defaultEase = Ease.Linear;
    [SerializeField] Material[] materials;
    private SkinnedMeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();

        PunchMaterial(0, 1f);
    }

    public void ChangeMaterial(int materialIndex, float duration, int changeMaterialNumber = 0, Ease ease = Ease.INTERNAL_Zero)
    {
        Material beforeMaterial = new Material(meshRenderer.materials[changeMaterialNumber]);
        Material afterMaterial = materials[materialIndex];

        if(ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        DOVirtual.Float(0f, 1f, duration, f =>
        {
            meshRenderer.materials[changeMaterialNumber].Lerp(beforeMaterial, afterMaterial, f);
        }).SetEase(ease);
    }

    public void ChangeMaterial(Material afterMaterial, float duration, int changeMaterialNumber = 0, Ease ease = Ease.INTERNAL_Zero)
    {
        Material beforeMaterial = new Material(meshRenderer.materials[changeMaterialNumber]);

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        DOVirtual.Float(0f, 1f, duration, f =>
        {
            meshRenderer.materials[changeMaterialNumber].Lerp(beforeMaterial, afterMaterial, f);
        }).SetEase(ease);
    }

    public void PunchMaterial(int materialIndex, float duration, int changeMaterialNumber = 0, Ease ease = Ease.INTERNAL_Zero)
    {
        Material normalMaterial = new Material(meshRenderer.materials[changeMaterialNumber]);
        Material punchMaterial = materials[materialIndex];

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        DOVirtual.Float(-1f, 1f, duration, f =>
        {
            float value = 1f - Mathf.Abs(f);
            meshRenderer.materials[changeMaterialNumber].Lerp(normalMaterial, punchMaterial, value);
        }).SetEase(ease);
    }

    public void PunchMaterial(Material punchMaterial, float duration, int changeMaterialNumber = 0, Ease ease = Ease.INTERNAL_Zero)
    {
        Material normalMaterial = new Material(meshRenderer.materials[changeMaterialNumber]);

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        DOVirtual.Float(-1f, 1f, duration, f =>
        {
            float value = 1f - Mathf.Abs(f);
            meshRenderer.materials[changeMaterialNumber].Lerp(normalMaterial, punchMaterial, value);
        }).SetEase(ease);
    }


}
