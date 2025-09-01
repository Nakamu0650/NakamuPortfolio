using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hanadayori/Item/CreateItem")]
public class G_ItemParameters : ScriptableObject
{
    //About CoolTimeChecker
    public enum CoolTime
    {
        False_CoolTime = 0,
        True_CoolTime = 1
    }


    [Tooltip("アイテム名")]
    public string itemName;
    [Tooltip("効果量")]
    public int effectSize;
    [Tooltip("クールタイム有無")]
    [SerializeField] public CoolTime coolTime;
}