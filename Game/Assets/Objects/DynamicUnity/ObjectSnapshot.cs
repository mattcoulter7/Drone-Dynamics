using System;
using System.Collections.Generic;
using System.Reflection;

public class ObjectSnapshot
{
    private Dictionary<string, object> __dict__ = new Dictionary<string, object>();
    private PropertyInfo[] properties;

    public ObjectSnapshot(object obj)
    {
        properties = obj.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            try
            {
                __dict__[property.Name] = property.GetValue(obj);
            }
            catch (Exception)
            {

            }
        }
    }

    public void Load(object obj)
    {
        foreach (PropertyInfo property in properties)
        {
            try
            {
                property.SetValue(obj, this[property.Name]);
            }
            catch (Exception)
            {

            }
        }
    }
    public object this[string propertyName]
    {
        get => __dict__[propertyName];
        set => __dict__[propertyName] = value;
    }
}