using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloomableArea", menuName = "Hanadayori/BloomSystem/BloomableArea")]
public class G_BloomSystemBloomableAreas : ScriptableObject
{
    public Vector3[] bloomablePositions;

    public G_BloomSystemBloomableAreas(Vector3[] bloomables)
    {
        bloomablePositions = bloomables;
    }
}
