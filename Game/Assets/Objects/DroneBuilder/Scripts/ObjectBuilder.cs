using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectBuilder : MonoBehaviour
{
    public GameObject obj;
    public GameObject canvasUI;
    public ComponentEditor componentEditor;

    public bool buildMode = false;

    private BuildableObject[] buildableObjects;
    private int selectedBuildableObjectIndex = 0;
    private void OnSelect()
    {
        BuildableObject buildableObject = GetSelectedBuildableObject();
        Component comp = buildableObject.GetSelectedComponent();

        // Update the component Editor to edit properties for this component
        componentEditor.Setup(
            comp
        );

        if (buildMode)
        {
            buildableObject.OnSelect();
        }
    }

    private void OnDeselect()
    {
        BuildableObject buildableObject = GetSelectedBuildableObject();
        Component comp = buildableObject.GetSelectedComponent();

        buildableObject.OnDeselect();
    }

    private void Start()
    {
        buildableObjects = obj.GetComponentsInChildren<BuildableObject>();
        HandleBuildModeState();
    }

    public BuildableObject GetSelectedBuildableObject()
    {
        return buildableObjects[selectedBuildableObjectIndex];
    }

    public void NextBuildableObjectComponent(InputAction.CallbackContext cc)
    {
        OnDeselect();

        GetSelectedBuildableObject().NextComponent();

        OnSelect();
    }
    public void PreviousBuildableObjectComponent(InputAction.CallbackContext cc)
    {
        OnDeselect();

        GetSelectedBuildableObject().PreviousComponent();

        OnSelect();
    }

    public void NextBuildableObject(InputAction.CallbackContext cc)
    {
        OnDeselect();

        selectedBuildableObjectIndex += 1;
        if (selectedBuildableObjectIndex > buildableObjects.Length - 1)
            selectedBuildableObjectIndex = 0;

        OnSelect();
    }

    public void PreviousBuildableObject(InputAction.CallbackContext cc)
    {
        OnDeselect();

        selectedBuildableObjectIndex -= 1;
        if (selectedBuildableObjectIndex < 0)
            selectedBuildableObjectIndex = buildableObjects.Length - 1;

        OnSelect();
    }

    public void ToggleBuildMode(InputAction.CallbackContext cc)
    {
        buildMode = !buildMode;
        HandleBuildModeState();
    }

    private void HandleBuildModeState()
    {
        if (buildMode)
        {
            OnSelect();
        }
        else
        {
            OnDeselect();
        }
        canvasUI.SetActive(buildMode);
    }


    private void OnEnable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            return;

        playerInput.actions["nextObject"].performed += NextBuildableObject;
        playerInput.actions["previousObject"].performed += PreviousBuildableObject;
        playerInput.actions["nextComponent"].performed += NextBuildableObjectComponent;
        playerInput.actions["previousComponent"].performed += PreviousBuildableObjectComponent;
        playerInput.actions["toggleBuildMode"].performed += ToggleBuildMode;
    }
    private void OnDisable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null) 
            return;

        playerInput.actions["nextObject"].performed -= NextBuildableObject;
        playerInput.actions["previousObject"].performed -= PreviousBuildableObject;
        playerInput.actions["nextComponent"].performed -= NextBuildableObjectComponent;
        playerInput.actions["previousComponent"].performed -= PreviousBuildableObjectComponent;
        playerInput.actions["toggleBuildMode"].performed -= ToggleBuildMode;
    }
}
