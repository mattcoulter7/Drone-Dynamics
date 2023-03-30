using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerFlightController : FlightController
{
    private InputAction yawInput;
    private InputAction throttleInput;
    private InputAction pitchInput;
    private InputAction rollInput;
    protected override void Awake()
    {
        base.Awake();

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        yawInput = playerInput.actions["yaw"];
        throttleInput = playerInput.actions["throttle"];
        pitchInput = playerInput.actions["pitch"];
        rollInput = playerInput.actions["roll"];
    }
    protected override void HandleInputs()
    {
        // left gyro
        leftGyro.y = throttleInput.ReadValue<float>();
        leftGyro.x = yawInput.ReadValue<float>();

        // right gyro
        rightGyro.y = pitchInput.ReadValue<float>();
        rightGyro.x = rollInput.ReadValue<float>();
    }
}
