using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


class InvalidTargetException : Exception
{
    public InvalidTargetException(string target, Type type) : base($"{target} does not exist as a Property, Field or Method on Type {type.Name}")
    {
    }
}


[Serializable]
public enum ParamType
{
    NULL = 0,
    FLOAT = 1,
    STRING = 2,
    INTEGER = 3,
    BOOLEAN = 4,
    OBJECT = 5,
    CUSTOM_GETTER = 6,
    INITIALISE_PARAM = 7
}

[Serializable]
public class InitialiseParam
{
    public string typeName;
    public ParamSet paramSet;
    public void OnInitialise()
    {
        paramSet.OnInitialise();
    }
    public object GetObject()
    {
        Type t = Type.GetType(typeName);

        return Activator.CreateInstance(t, paramSet.GetParams());
    }
}


[Serializable]
public class ConfigurableParam
{
    public ParamType paramType;

    public float floatParameter;
    public string stringParameter;
    public int intParameter;
    public bool boolParameter;
    public DynamicMemberInfoChain customGetter;
    public UnityEngine.Object objectParameter;
    public InitialiseParam initialiseParameter;
    public void OnInitialise()
    {
        switch (paramType)
        {
            case ParamType.CUSTOM_GETTER:
                customGetter.OnInitialise();
                break;
            case ParamType.INITIALISE_PARAM:
                initialiseParameter.OnInitialise();
                break;
            default:
                break;
        }
    }

    public object GetParameter()
    {
        switch (paramType)
        {
            case ParamType.NULL:
                return null;
            case ParamType.FLOAT:
                return floatParameter;
            case ParamType.BOOLEAN:
                return boolParameter;
            case ParamType.INTEGER:
                return intParameter;
            case ParamType.OBJECT:
                return objectParameter;
            case ParamType.STRING:
                return stringParameter;
            case ParamType.CUSTOM_GETTER:
                return customGetter.GetValue();
            case ParamType.INITIALISE_PARAM:
                return initialiseParameter.GetObject();
            default:
                return null;
        }
    }
}

[Serializable]
public class ParamSet
{
    public ConfigurableParam[] parameters;
    public ParamSet(ConfigurableParam[] parameters = null)
    {
        if (parameters == null)
            parameters = new ConfigurableParam[] { };

        this.parameters = parameters;
    }
    public void OnInitialise()
    {
        foreach (ConfigurableParam param in parameters)
        {
            param.OnInitialise();
        }
    }
    public object[] GetParams()
    {
        return parameters.Select(p => p.GetParameter()).ToArray();
    }
}

[Serializable]
public struct TargetConfig
{
    public TargetConfig(string name, ParamSet paramSet = null, int targetSetIndex = 0)
    {
        this.name = name;
        if (paramSet == null)
            paramSet = new ParamSet();
        this.paramSet = paramSet;
        this.targetSetIndex = targetSetIndex;
    }
    public string name; // chain which which direct compiler to the property/method 
    public ParamSet paramSet; // method path which returns the current value
    public int targetSetIndex; // when calling SetValue on DynamicModifier, what parameter index does the new value belong
}

public interface IMemberInfo
{
    public object Invoke();
    public Type GetReturnType();
    public object GetValue();
    public void SetValue(object value);
    public string GetName();
    public bool IsDefined(Type attrType);
    public Attribute GetCustomAttribute(Type attrType);
    public IEnumerable<Attribute> GetCustomAttributes();
}

public class DynamicMemberInfo : IMemberInfo
{
    public TargetConfig targetConfig;
    public object parent;
    public IMemberInfo parentInfo;

    private object GetParent() => parentInfo == null ? parent : parentInfo.GetValue();
    private Type parentType;
    private PropertyInfo propertyInfo;
    private FieldInfo fieldInfo;
    private MethodInfo methodInfo;
    private MemberInfo memberInfo;

    public DynamicMemberInfo(object parent, TargetConfig targetConfig, IMemberInfo parentInfo = null)
    {
        this.targetConfig = targetConfig;
        this.parent = parent;
        this.parentInfo = parentInfo;

        parentType = GetParent().GetType();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        propertyInfo = parentType.GetProperty(this.targetConfig.name, flags);
        if (propertyInfo == null)
        {
            fieldInfo = parentType.GetField(this.targetConfig.name, flags);
            if (fieldInfo == null)
            {
                MethodInfo[] methods = parentType.GetMethods(flags);
                try
                {
                    methodInfo = parentType.GetMethods().First(m => m.Name == this.targetConfig.name && m.GetParameters().Length == targetConfig.paramSet.parameters.Length);
                }
                catch (InvalidOperationException) { }
            }
        }

        memberInfo = GetMemberInfo();
    }

    public object Invoke()
    {
        return GetValue();
    }

    public Type GetReturnType()
    {
        // DETERMINE THE NEW TYPE
        if (propertyInfo != null)
            return propertyInfo.PropertyType;
        if (fieldInfo != null)
            return fieldInfo.FieldType;
        if (methodInfo != null)
            return methodInfo.ReturnType;
        throw new InvalidTargetException(this.targetConfig.name, parentType);
    }
    public object GetValue()
    {
        object p = GetParent();
        if (propertyInfo != null)
            return propertyInfo.GetValue(p, null);
        if (fieldInfo != null)
            return fieldInfo.GetValue(p);
        if (methodInfo != null)
            return methodInfo.Invoke(p, targetConfig.paramSet.GetParams());
        throw new InvalidTargetException(this.targetConfig.name, parentType);
    }

    public void SetValue(object value)
    {
        object p = GetParent();
        if (propertyInfo != null)
            propertyInfo.SetValue(p, value);
        else if (fieldInfo != null)
            fieldInfo.SetValue(p, value);
        else if (methodInfo != null)
        {
            object[] parameters = targetConfig.paramSet.GetParams();
            if (targetConfig.targetSetIndex <= parameters.Length - 1) 
                parameters[targetConfig.targetSetIndex] = value;
            methodInfo.Invoke(p, parameters);
        }
        else
            throw new InvalidTargetException(targetConfig.name, parentType);
    }

    public string GetName()
    {
        return memberInfo.Name;
    }

    public bool IsMethod()
    {
        return methodInfo != null;
    }

    public bool IsDefined(Type attrType)
    {
        return Attribute.IsDefined(memberInfo, attrType);
    }
    public Attribute GetCustomAttribute(Type attrType)
    {
        foreach (var attribute in memberInfo.GetCustomAttributes())
        {
            if (attrType.IsAssignableFrom(attribute.GetType()))
            {
                return attribute;
            }
        }
        return null;
    }
    public IEnumerable<Attribute> GetCustomAttributes() { 
        return memberInfo.GetCustomAttributes();
    }

    public MemberInfo GetMemberInfo()
    {
        if (propertyInfo != null)
            return propertyInfo;
        if (fieldInfo != null)
            return fieldInfo;
        if (methodInfo != null)
            return methodInfo;

        throw new InvalidTargetException(targetConfig.name, parentType);
    }
}

[Serializable]
public class DynamicMemberInfoChain : IMemberInfo
{
    public UnityEngine.Object objectReference; // component for where the chain attaches to
    public TargetConfig[] targets; // chain which which direct compiler to the property/method 

    private List<DynamicMemberInfo> chainTargets;

    public DynamicMemberInfoChain(UnityEngine.Object objectReference, TargetConfig[] targets)
    {
        this.objectReference = objectReference;
        this.targets = targets;

        OnInitialise();
    }
    public void OnInitialise()
    {
        chainTargets = new List<DynamicMemberInfo>();

        object parent = objectReference;
        IMemberInfo parentTargetInfo = null;
            
        // create TargetInfo objects for each chain link which allow us to easily get/set/call the property/method etc.
        for (int i = 0; i < targets.Length; i++)
        {
            TargetConfig target = targets[i];

            // store the target info reference
            target.paramSet.OnInitialise();
            DynamicMemberInfo targetInfo = new DynamicMemberInfo(parent, target, parentTargetInfo);
            parentTargetInfo = targetInfo;

            chainTargets.Add(targetInfo);
        }
    }
    public object Invoke() => GetTargetInfo().Invoke();
    public Type GetReturnType() => GetTargetInfo().GetReturnType();
    public object GetValue() => GetTargetInfo().GetValue();
    public void SetValue(object value) => GetTargetInfo().SetValue(value);
    public string GetName() => GetTargetInfo().GetName();
    public bool IsDefined(Type attrType) => GetTargetInfo().IsDefined(attrType);
    public Attribute GetCustomAttribute(Type attrType) => GetTargetInfo().GetCustomAttribute(attrType);
    public IEnumerable<Attribute> GetCustomAttributes() => GetTargetInfo().GetCustomAttributes();
    public DynamicMemberInfo GetTargetInfo() => chainTargets[chainTargets.Count - 1];
    public DynamicMemberInfoChain Link(params TargetConfig[] targets)
    {
        // Copy the config into a new object
        return new DynamicMemberInfoChain(
            objectReference,
            this.targets.Concat(targets).ToArray()
        );
    }
}
