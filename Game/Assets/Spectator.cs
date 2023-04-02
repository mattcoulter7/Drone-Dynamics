using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Spectator : MonoBehaviour
{
    private int objectIndex = -1;
    private ICameraFollow[] followObjects;
    private bool spectating = false;
    private Transform target = null;
    private void Start()
    {
        followObjects = GetComponents<ICameraFollow>();
    }
    private Transform GetNextTarget()
    {
        GameObject[] spectateObjects = GameObject.FindGameObjectsWithTag("Multiplayer");
        if (spectateObjects.Length == 0) return null;

        objectIndex++;

        // reset index if it is out of range
        if (objectIndex > spectateObjects.Length - 1)
        {
            objectIndex = 0;
        }

        return spectateObjects[objectIndex].transform; 
    }

    private void ToggleSpectate(InputAction.CallbackContext cc)
    {
        target = GetNextTarget();
        if (target == null) return;

        spectating = !spectating;
        if (spectating)
        {
            StartSpectate(cc);
        }
        else
        {
            StopSpectate(cc);
        }
    }

    private void StartSpectate(InputAction.CallbackContext cc)
    {
        if (target == null) return;

        foreach (ICameraFollow followObject in followObjects)
        {
            followObject.target = GetNextTarget();
        }
    }

    private void StopSpectate(InputAction.CallbackContext cc)
    {
        foreach (ICameraFollow followObject in followObjects)
        {
            followObject.ReturnToOriginalTarget();
        }
    }
    private void OnEnable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null) return;

        InputAction spectate = playerInput.actions["spectate"];

        // spectate.started += StartSpectate;
        // spectate.canceled += StopSpectate;

        spectate.performed += ToggleSpectate;
    }
    private void OnDisable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null) return;

        InputAction spectate = playerInput.actions["spectate"];
        // spectate.started -= StartSpectate;
        // spectate.canceled -= StopSpectate;
        
        spectate.performed -= ToggleSpectate;
    }
}
