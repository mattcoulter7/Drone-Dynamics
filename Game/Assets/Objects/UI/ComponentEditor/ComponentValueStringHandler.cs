using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueStringHandler : ComponentValueHandler
{
    private TMP_InputField inputField;

    protected override void SetInitialValue(object value)
    {
        inputField.text = (string)value;
    }

    protected override void GetUIElements()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
    }
    public void OnChangeString(string value)
    {
        OnChange(value);
    }
}
