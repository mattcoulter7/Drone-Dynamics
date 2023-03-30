using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueIntHandler : ComponentValueHandler
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
    public void OnChangeInt(int value)
    {
        OnChange(value);
    }
}
