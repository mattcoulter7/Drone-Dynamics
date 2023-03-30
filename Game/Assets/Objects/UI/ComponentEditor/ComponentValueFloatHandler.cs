using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueFloatHandler : ComponentValueHandler
{
    private Slider sliderField;
    private TMP_InputField inputField;

    private RangeAttribute rangeAttribute = null;
    public void OnChangeFloat(float value)
    {
        OnChange(value);
    }
    public void OnChangeString(string value)
    {
        try
        {
            float f = float.Parse(value, System.Globalization.NumberStyles.Float);
            OnChangeFloat(f);
        }
        catch (Exception)
        {
            SetInitialValue(memberInfo.GetValue());
        }
    }
    protected override object Validate(object value)
    {
        if (rangeAttribute == null) return value;
        if ((float)value < rangeAttribute.min)
            return rangeAttribute.min;
        if ((float)value > rangeAttribute.max)
            return rangeAttribute.max;
        return value;
    }
    protected override void SetInitialValue(object value)
    {
        if (sliderField != null)
            sliderField.value = (float)value;
        if (inputField != null)
            inputField.text = value.ToString();
    }

    protected override void HandleAttribute(Attribute attr)
    {
        switch (attr)
        {
            case RangeAttribute rangeAttribute:
                this.rangeAttribute = rangeAttribute;
                if (sliderField != null)
                {
                    sliderField.minValue = rangeAttribute.min;
                    sliderField.maxValue = rangeAttribute.max;
                }
                break;
        }
    }

    protected override void GetUIElements()
    {
        sliderField = GetComponentInChildren<Slider>();
        inputField = GetComponentInChildren<TMP_InputField>();
    }
}
