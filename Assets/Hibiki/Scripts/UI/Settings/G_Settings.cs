using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class G_Settings : MonoBehaviour
{
    [SerializeField] protected string mainTitle = "Title";
    [HideInInspector]public float value = 0;
    [SerializeField]public UnityEvent onValueChanged;
    [SerializeField] public UnityEngine.UI.Selectable selectable;

    public int GetIntValue()
    {
        return Mathf.RoundToInt(value);
    }

    public float GetFloatValue()
    {
        return value;
    }

    public bool GetBoolValue()
    {
        return Convert.ToBoolean(value);
    }

    public virtual void SetIntValue(int set)
    {
        value = (float)set;
    }

    public virtual void SetFloatValue(float set)
    {
        value = set;
    }

    public virtual void SetBoolValue(bool set)
    {
        value = Convert.ToSingle(set);
    }
}
