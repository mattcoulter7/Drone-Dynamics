using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComponentEditor : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private Transform parent;
    [SerializeField] private float elementPadding = 15f;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject headerPrefab;
    [SerializeField] private GameObject booleanPrefab;
    [SerializeField] private GameObject floatPrefab;
    [SerializeField] private GameObject floatRangePrefab;
    [SerializeField] private GameObject stringPrefab;
    [SerializeField] private GameObject intPrefab;
    [SerializeField] private GameObject vector3Prefab;
    [SerializeField] private GameObject vector2Prefab;

    private Dictionary<Type, GameObject> uiPrefabs = new Dictionary<Type, GameObject>();

    private Component component;
    private List<GameObject> uiInstances = new List<GameObject>();
    private void Start()
    {
        uiPrefabs[typeof(bool)] = booleanPrefab;
        uiPrefabs[typeof(float)] = floatPrefab;
        uiPrefabs[typeof(string)] = stringPrefab;
        uiPrefabs[typeof(int)] = intPrefab;
        uiPrefabs[typeof(Vector3)] = vector3Prefab;
        uiPrefabs[typeof(Vector2)] = vector2Prefab;
    }
    public void Setup(Component component)
    {
        this.component = component;
        Dictionary<string, DynamicMemberInfoChain> modifiers = new Dictionary<string, DynamicMemberInfoChain>();

        foreach (GameObject go in uiInstances)
        {
            Destroy(go);
        }
        uiInstances.Clear();

        // Get the type of the Component
        var componentType = this.component.GetType();
        IEnumerable<Type> validTypes = GetUserCreatedTypes(componentType);

        // Get all fields and properties, including private ones
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        // Filter out fields and properties that should not be serialized
        foreach (var field in componentType.GetFields(flags))
        {
            if (!field.CanWrite() || field.IsNotSerialized) continue;
            if (!validTypes.Contains(field.DeclaringType)) continue;
            if (field.Name.Contains("_")) continue;
            modifiers[field.Name] = new DynamicMemberInfoChain(
                this.component,
                new TargetConfig[]{
                    new TargetConfig(field.Name)
                }
            );
        }
        foreach (var property in componentType.GetProperties(flags))
        {
            if (!property.CanWrite || !property.CanRead || property.GetIndexParameters().Length > 0) continue;
            if (!validTypes.Contains(property.DeclaringType)) continue;
            if (property.Name.Contains("_")) continue;
            modifiers[property.Name] = new DynamicMemberInfoChain(
                this.component,
                new TargetConfig[]{
                    new TargetConfig(property.Name)
                }
            );
        }

        // Iterate through all public properties of the component
        float heightOffset = elementPadding;
        foreach (var pair in modifiers)
        {
            GameObject uiFieldIntance = SetupUIField(pair.Value);
            if (uiFieldIntance == null)
                continue;

            RectTransform uiFieldIntanceTransform = uiFieldIntance.GetComponent<RectTransform>();
            uiFieldIntanceTransform.anchoredPosition += new Vector2(0, heightOffset);

            uiInstances.Add(uiFieldIntance);
            heightOffset = uiFieldIntanceTransform.anchoredPosition.y + elementPadding;
        }
    }

    private IEnumerable<Type> GetUserCreatedTypes(Type T)
    {
        // 1. Determine which class are valid for the provided type
        List<Type> validTypes = new List<Type>();
        while (T != typeof(MonoBehaviour))
        {
            validTypes.Add(T);
            T = T.BaseType;
        }

        return validTypes;
    }

    private GameObject SetupUIField(DynamicMemberInfoChain info)
    {
        GameObject uiFieldPrefab = GetUIPrefabFromInfo(info);
        if (uiFieldPrefab == null)
            return null;


        GameObject uiFieldIntance = Instantiate(uiFieldPrefab, parent);

        ComponentValueHandler handler = uiFieldIntance.GetComponent<ComponentValueHandler>();
        handler.Setup(info);

        return uiFieldIntance;
    }

    private GameObject GetUIPrefabFromInfo(IMemberInfo info)
    {
        // Need a return type to determine what the stored value is
        Type type = info.GetReturnType();
        if (type == null)
            return null;

        // various conditions which can also influence the prefab
        if (floatRangePrefab != null && type == typeof(float) && info.IsDefined(typeof(RangeAttribute)))
            return floatRangePrefab;

        // Get the field type from the configured prefabs
        GameObject prefab;
        uiPrefabs.TryGetValue(type, out prefab);
        return prefab;
    }
}
