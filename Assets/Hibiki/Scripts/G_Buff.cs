using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Hanadayori/BuffSystem")]
public class G_Buff : ScriptableObject
{
    public string buffName;
    public int id;
    public bool isUnlimited = false;
    [Range(0f,99f)]public float applyTime = 0f;
    [Range(0f, 999f)] public float value = 0f;

    [HideInInspector]public float time;

    private void OnValidate()
    {
        time = applyTime;
        id = Random.Range(10000, 100000);
    }

    /// <summary>
    /// Use buff which is unlimited.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    public G_Buff(string _name, out int _id, float _value)
    {
        buffName = _name;
        id = Random.Range(10000, 100000);
        _id = id;
        value = _value;
        isUnlimited = true;
    }

    /// <summary>
    /// Use buff which is limited.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    /// <param name="_time"></param>
    public G_Buff(string _name, out int _id, float _value, float _time)
    {
        float apply = Mathf.Clamp(_time, 0f, 99f);
        buffName = _name;
        id = Random.Range(10000, 100000);
        _id = id;
        value = _value;
        time = apply;
        isUnlimited = false;
    }
}
