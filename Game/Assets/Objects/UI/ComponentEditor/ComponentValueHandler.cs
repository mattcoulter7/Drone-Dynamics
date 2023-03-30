using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public abstract class ComponentValueHandler : MonoBehaviour
{
    protected DynamicMemberInfoChain memberInfo = null;

    private ComponentValueHandler parent = null;
    private Text label = null;

    public void Setup(
        DynamicMemberInfoChain info,
        ComponentValueHandler parent = null
    )
    {
        this.parent = parent;
        this.memberInfo = info;
        name = info.GetName();

        GetUIElements();
        SetupAdditionalHandlers();
        SetInitialValue(
            info.GetValue()
        );
        UpdateLabel();
        HandleAttributes();
    }
    public virtual void OnChange(string property, object value)
    {
        if (parent != null)
            parent.OnChange(name, value);
        else
            memberInfo.SetValue(value);
    }
    public virtual void OnChange(object value)
    {
        value = Validate(value);
        OnChange(memberInfo.GetName(), value);
    }
    protected virtual object Validate(object value) { return value; }
    protected virtual void SetInitialValue(object value) { }
    protected virtual void SetupAdditionalHandlers() { }
    protected virtual void GetUIElements() { }
    protected virtual void HandleAttribute(Attribute attr) { }

    private void HandleAttributes()
    {
        foreach (Attribute attr in memberInfo.GetCustomAttributes())
        {
            HandleAttribute(attr);
        }
    }
    private void UpdateLabel()
    {
        Transform labelTransform = transform.Find("Label");
        if (labelTransform == null) return;

        label = labelTransform.GetComponent<Text>();
        if (label == null) return;

        label.text = memberInfo.GetName();
    }
}
