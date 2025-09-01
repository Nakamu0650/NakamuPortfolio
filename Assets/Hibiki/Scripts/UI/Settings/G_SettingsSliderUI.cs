using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class G_SettingsSliderUI : G_Settings
{
#if UNITY_EDITOR
    [SerializeField] float minValue = 0f, maxValue = 1f;
    [SerializeField] bool wholeNumbers = false;

    [Space(20)]
#endif

    [SerializeField] UI ui;

    [Serializable] private class UI
    {
        public Slider slider;
        public TMP_Text titleText;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ui.titleText.text = mainTitle;

        ui.slider.wholeNumbers = wholeNumbers;
        ui.slider.minValue = minValue;
        ui.slider.maxValue = maxValue;

        value = ui.slider.value;
    }
#endif

    public void OnValueChange()
    {
        value = ui.slider.value;
        onValueChanged.Invoke();
    }

    public override void SetFloatValue(float set)
    {
        base.SetFloatValue(set);
        ui.slider.value = value;
    }

    public override void SetIntValue(int set)
    {
        base.SetIntValue(set);
        ui.slider.value = value;
    }
}
