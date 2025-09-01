using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_DamageEffectGenerator : MonoBehaviour
{
    public static G_DamageEffectGenerator instance;
    [SerializeField] GameObject effectPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void OnDamage(Vector3 hit)
    {
        Instantiate(effectPrefab, hit, Quaternion.identity);
    }
}
