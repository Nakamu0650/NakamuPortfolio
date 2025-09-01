using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class G_BlendShapeChanger : MonoBehaviour
{
    [SerializeField] Ease defaultEase = Ease.Linear;
    [SerializeField] SerializedDictionary<float[]> blendShapeTempletes;
    private SkinnedMeshRenderer meshRenderer;

    private int blendShapesAmount;

    private bool isTweening;

    private Tween lastTween;

    private void OnValidate()
    {
        if (blendShapeTempletes != null)
        {
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (!meshRenderer)
            {
                return;
            }
            int amount = meshRenderer.sharedMesh.blendShapeCount;

            foreach (var dict in blendShapeTempletes)
            {
                if(dict.Value.Length != amount)
                {
                    float[] array = dict.Value;
                    Array.Resize(ref array, amount);
                    blendShapeTempletes[dict.Key] = array;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        lastTween = null;
        isTweening = false;
        blendShapesAmount = meshRenderer.sharedMesh.blendShapeCount;
    }


    public void ChangeBlendShapeAsNew(float[] afterShapes, float duration, Ease ease = Ease.INTERNAL_Zero, bool killBefore = false)
    {
        float[] beforeShapes = GetBlendShapes();

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        StartCoroutine(Change(beforeShapes, afterShapes, duration, ease, killBefore));
    }

    public void ChangeBlendShape(string shapeName, float duration, Ease ease = Ease.INTERNAL_Zero, bool killBefore = false)
    {
        float[] beforeShapes = GetBlendShapes();
        float[] afterShapes = blendShapeTempletes[shapeName];

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }
        StartCoroutine(Change(beforeShapes, afterShapes, duration, ease, killBefore));
    }

    public void PunchBlendShapeAsNew(float[] afterShapes, float duration, Ease ease = Ease.INTERNAL_Zero, bool killBefore = false)
    {
        float[] beforeShapes = GetBlendShapes();

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        StartCoroutine(Punch(beforeShapes, afterShapes, duration, ease, killBefore));
    }

    public void PunshBlendShape(string shapeName, float duration, Ease ease = Ease.INTERNAL_Zero, bool killBefore = false)
    {
        float[] beforeShapes = GetBlendShapes();
        float[] afterShapes = blendShapeTempletes[shapeName];

        if (ease == Ease.INTERNAL_Zero)
        {
            ease = defaultEase;
        }

        StartCoroutine(Punch(beforeShapes, afterShapes, duration, ease, killBefore));
    }

    public void SetBlendShape(string shapeName)
    {
        SetBlendShape(blendShapeTempletes[shapeName]);
    }

    public void SetBlendShape(float[] shapes)
    {
        for(int i = 0; i < blendShapesAmount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, shapes[i]);
        }
    }


    private IEnumerator Change(float[] beforeShapes, float[] afterShapes, float duration, Ease ease, bool killBefore)
    {
        if (killBefore)
        {
            KillTween();
        }
        else
        {
            yield return new WaitUntil(() => !isTweening);
        }
        var doo =  DOVirtual.Float(0f, 1f, duration, f =>
        {
            for (int i = 0; i < blendShapesAmount; i++)
            {
                if (beforeShapes[i] == afterShapes[i])
                {
                    continue;
                }
                meshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(beforeShapes[i], afterShapes[i], f));
            }
        }).SetEase(ease);
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator Punch(float[] beforeShapes, float[] afterShapes, float duration, Ease ease, bool killBefore)
    {
        if (killBefore)
        {
            KillTween();
        }
        else
        {
            yield return new WaitUntil(() => !isTweening);
        }
        DOVirtual.Float(-1f, 1f, duration, f =>
        {
            float value = 1f - Mathf.Abs(f);
            for (int i = 0; i < blendShapesAmount; i++)
            {
                if (beforeShapes[i] == afterShapes[i])
                {
                    continue;
                }
                meshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(beforeShapes[i], afterShapes[i], value));
            }
        }).SetEase(ease);
        yield return new WaitForSeconds(duration);
    }

    public float[] GetBlendShapes()
    {
        List<float> _blendShapes = new List<float>();
        for(int i = 0; i< blendShapesAmount; i++)
        {
            _blendShapes.Add(meshRenderer.GetBlendShapeWeight(i));
        }

        return _blendShapes.ToArray();
    }

    private void KillTween()
    {
        if(lastTween == null)
        {
            return;
        }
        lastTween.Kill();
    }
}