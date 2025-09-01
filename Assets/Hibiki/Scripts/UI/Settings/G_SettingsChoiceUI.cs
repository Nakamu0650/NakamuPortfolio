using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class G_SettingsChoiceUI : G_Settings
{
#if UNITY_EDITOR
    [SerializeField] bool trueOrFalse = false;
    [SerializeField]
    List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>()
    {
        new TMP_Dropdown.OptionData {text="OptionA"}, new TMP_Dropdown.OptionData { text = "OptionB" }, new TMP_Dropdown.OptionData { text = "OptionC" }
    };


    [Space(20)]
#endif

    [SerializeField] UI ui;

    [Serializable] private class UI
    {
        public TMP_Dropdown dropdown;
        public TMP_Text titleText;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (trueOrFalse)
        {
            options = new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData {text="Off"}, new TMP_Dropdown.OptionData { text = "On" }
            };
        }

        ui.titleText.text = mainTitle;
        ui.dropdown.options = options;
    }
#endif

    public void OnValueChange()
    {
        value = ui.dropdown.value;
        onValueChanged.Invoke();
    }

    public override void SetBoolValue(bool set)
    {
        base.SetBoolValue(set);
        ui.dropdown.value = Mathf.RoundToInt(value);
    }

    public override void SetIntValue(int set)
    {
        base.SetIntValue(set);
        ui.dropdown.value = Mathf.RoundToInt(value);
    }


}
