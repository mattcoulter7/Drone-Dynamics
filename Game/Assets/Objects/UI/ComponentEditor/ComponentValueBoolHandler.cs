using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueBoolHandler : ComponentValueHandler
{
    private Toggle toggleField;

    protected override void SetInitialValue(object value)
    {
        toggleField.isOn = (bool)value;
    }

    protected override void GetUIElements()
    {
        toggleField = GetComponentInChildren<Toggle>();
    }
    public void OnChangeBool(bool value)
    {
        OnChange(value);
    }
}
