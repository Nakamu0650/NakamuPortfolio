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


    [Tooltip("�A�C�e����")]
    public string itemName;
    [Tooltip("���ʗ�")]
    public int effectSize;
    [Tooltip("�N�[���^�C���L��")]
    [SerializeField] public CoolTime coolTime;
}