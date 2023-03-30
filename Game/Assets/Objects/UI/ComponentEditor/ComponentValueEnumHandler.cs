using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueEnumHandler : ComponentValueHandler
{
    private TMP_Dropdown dropdownField;

    protected override void SetInitialValue(object value)
    {
        dropdownField.value = (int)value;
    }

    protected override void GetUIElements()
    {
        dropdownField = GetComponentInChildren<TMP_Dropdown>();
    }
    public void OnChangeEnum(int value)
    {
        TMP_Dropdown.OptionData option = dropdownField.options[value];
        string text = option.text;
        OnChange(text);
    }
}
