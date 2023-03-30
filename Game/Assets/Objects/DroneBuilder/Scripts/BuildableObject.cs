using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    public Component[] buildableComponents;
    public Renderer representitiveObjectRenderer;
    public Material selectedMaterial;

    private int selectedComponentIndex = 0;
    private Material originalMaterial;

    private void Awake()
    {
        originalMaterial = representitiveObjectRenderer.material;
    }
    public Component GetSelectedComponent()
    {
        return buildableComponents[selectedComponentIndex];
    }

    public void NextComponent()
    {
        selectedComponentIndex += 1;
        if (selectedComponentIndex > buildableComponents.Length - 1)
        {
            selectedComponentIndex = 0;
        }
    }

    public void PreviousComponent()
    {
        selectedComponentIndex -= 1;
        if (selectedComponentIndex < 0)
        {
            selectedComponentIndex = buildableComponents.Length - 1;
        }
    }

    public void OnSelect()
    {
        representitiveObjectRenderer.material = selectedMaterial;
    }

    public void OnDeselect()
    {
        representitiveObjectRenderer.material = originalMaterial;
    }
}
