using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentValueVector3Handler : ComponentValueHandler
{
    protected override void SetupAdditionalHandlers()
    {
        base.SetupAdditionalHandlers();

        string[] fields = new string[] { "x", "y", "z" };

        ComponentValueFloatHandler[] floatHandlers = GetComponentsInChildren<ComponentValueFloatHandler>();
        for (int i = 0; i < floatHandlers.Length; i++)
        {
            var floatHandler = floatHandlers[i];

            floatHandler.Setup(
                memberInfo.Link(new TargetConfig(fields[i])),
                this
            );
        }
    }
    public override void OnChange(string property, object value)
    {
        Vector3 currentValue = (Vector3)memberInfo.GetValue();
        // Called when the direct value of the property is changed
        if (property == "x")
            currentValue.x = (float)value;
        else if (property == "y")
            currentValue.y = (float)value;
        else if (property == "z")
            currentValue.z = (float)value;

        base.OnChange(memberInfo.GetName(), currentValue);
    }
}
