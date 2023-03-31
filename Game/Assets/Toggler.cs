using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Toggler : MonoBehaviour
{
    public UnityEvent toggleOn;
    public UnityEvent toggleOff;
    public string inputKey;
    private bool toggled = false;
    private void Toggle(InputAction.CallbackContext cc)
    {
        toggled = !toggled;
        if (toggled)
        {
            toggleOn.Invoke();
        } 
        else
        {
            toggleOff.Invoke();
        }
    }
    private void OnEnable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            return;

        playerInput.actions[inputKey].performed += Toggle;
    }
    private void OnDisable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            return;

        playerInput.actions[inputKey].performed -= Toggle;
    }

}
