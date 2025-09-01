using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hanadayori/Player/PlayerParameter")]
public class P_PlayerParam : ScriptableObject
{
    public string playerName;
    public int HP;
    public int ATK;
    public int DEF;
}
