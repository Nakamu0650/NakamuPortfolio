using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class G_FlowerTrigger : MonoBehaviour
{
    private float nowRadius;
    private Vector3 roundPosition;

    public G_FlowerEnergyReceiver receiver;
    [SerializeField] LayerMask flowerLayer;
    [SerializeField] float defaultRadius=5f;
    [SerializeField] float radiusChangeSpeed = 5f;
    [SerializeField] G_BuffSystem buffSystem;



    private void Awake()
    {
        nowRadius = defaultRadius;
    }


    void FixedUpdate()
    {
        if (!G_FlowerBloomSystem.isAnalysed) return;

        UpdateBuffs();

        UpdateBloomArea();
    }


    /// <summary>
    /// Update buff times
    /// </summary>
    private void UpdateBuffs()
    {
        float size = buffSystem.GetValue(G_BuffSystem.BuffType.GrowRange) * defaultRadius;
        nowRadius = Mathf.Lerp(nowRadius, size, Time.fixedDeltaTime * radiusChangeSpeed);
    }


    /// <summary>
    /// If player moves beyond a certain level, make flowers bloom in the area.
    /// </summary>
    private void UpdateBloomArea()
    {
        Vector3Int pos = Vector3Int.RoundToInt(transform.position);
        if (pos != roundPosition)
        {
            roundPosition = pos;
            var cols = Physics.OverlapSphere(transform.position, nowRadius, flowerLayer);
            foreach (var col in cols)
            {
                G_FlowerGroup.flowerGroups[col.gameObject].Bloom();
            }
        }
    }
}
