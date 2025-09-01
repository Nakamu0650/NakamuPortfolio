using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class G_BuffSystem : MonoBehaviour
{
    public static G_BuffSystem instance;
    public enum BuffType
    {
        MoveSpeed, GrowRange, Defence, AttackStrength, AttackSpeed
    }

    private Dictionary<BuffType, List<G_Buff>> buffs;
    public BuffEvent onAddBuff = new BuffEvent(), onRemoveBuff = new BuffEvent();

    private void Awake()
    {
        instance = this;
        ClearBuffAll();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(BuffType type in Enum.GetValues(typeof(BuffType)))
        {
            if (buffs[type].Count == 0) continue;
            foreach(var buff in buffs[type])
            {
                if (!buff.isUnlimited)
                {
                    buff.time -= Time.deltaTime;
                }
            }
            var removes = buffs[type].Where(buff => (!buff.isUnlimited && (buff.time < 0f))).ToList();
            foreach(var remove in removes)
            {
                buffs[type].Remove(remove);
                onRemoveBuff.Invoke(type, remove);
            }
        }
    }

    /// <summary>
    /// Get total buff value.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetValue(BuffType type)
    {
        float value = (buffs[type].Count == 0)? 1f : buffs[type].Select(buff => buff.value).Aggregate((now, next) => now * next);
        return value;
    }

    public int GetValueToInt(BuffType type)
    {
        return Mathf.RoundToInt(GetValue(type));
    }

    /// <summary>
    /// Add buff
    /// </summary>
    /// <param name="type"></param>
    /// <param name="buff"></param>
    public void AddBuff(BuffType type, G_Buff buff)
    {
        buffs[type].Add(buff);
        onAddBuff.Invoke(type, buff);
    }

    /// <summary>
    /// Clear buff in select type
    /// </summary>
    /// <param name="type"></param>
    public void ClearBuff(BuffType type)
    {
        buffs[type].Clear();
    }

    public void ClearBuffAll()
    {
        buffs = new Dictionary<BuffType, List<G_Buff>>();

        foreach (BuffType type in Enum.GetValues(typeof(BuffType)))
        {
            buffs.Add(type, new List<G_Buff>());
        }
    }

    public void DeleteBuffFromName(BuffType type, string name)
    {
        var removes = buffs[type].Where(buff => (buff.name == name)).ToList();
        foreach(var remove in removes)
        {
            buffs[type].Remove(remove);
            onRemoveBuff.Invoke(type, remove);
        }
        
    }

    public void DeleteBuffFromId(BuffType type, int id)
    {
        var removes = buffs[type].Where(buff => (buff.id == id)).ToList();
        foreach (var remove in removes)
        {
            buffs[type].Remove(remove);
            onRemoveBuff.Invoke(type, remove);
        }
    }


}

public class BuffEvent : UnityEngine.Events.UnityEvent<G_BuffSystem.BuffType, G_Buff> { }
