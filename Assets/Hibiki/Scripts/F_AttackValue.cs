using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;

[CreateAssetMenu(menuName = "Hanadayori/AttackValue")]
public class F_AttackValue : ScriptableObject
{
    public string teckniicalName;
    public Attack[] attackValues = new Attack[1];
    public bool penetration = false;

    private void OnValidate()
    {
        foreach(Attack attack in attackValues)
        {
            attack.strengths.SetAll();
        }
    }

    [Serializable]
    public class Attack
    {
        public DifficultyValue<int> strengths = new DifficultyValue<int>();
        public int strength { get { return strengths.GetValue(); } }
        [Range(0f, 1f)] public float pushThreshould = 0f;
        [Range(0f, 100f)] public float pushPower = 10f;
    }
}