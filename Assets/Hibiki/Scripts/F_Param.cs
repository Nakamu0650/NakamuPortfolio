using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hanadayori/Parameter")]
public class F_Param : ScriptableObject
{
    public string characterName;
    public SerializedDictionary<GameManager.Difficulty, Param> param;

    private void OnValidate()
    {
        foreach(GameManager.Difficulty d in Enum.GetValues(typeof(GameManager.Difficulty)))
        {
            if (!param.ContainsKey(d))
            {
                param.Add(d, new Param());
            }
        }
    }


    public int GetHP()
    {
        return param[GameManager.instance.difficulty].HP;
    }

    public int GetATK()
    {
        return param[GameManager.instance.difficulty].ATK;
    }

    public int GetDEF()
    {
        return param[GameManager.instance.difficulty].DEF;
    }

    [Serializable]
    public class Param
    {
        public int HP;
        public int ATK;
        public int DEF;
    }
}
