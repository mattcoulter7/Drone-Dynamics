using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardFlightController : FlightController
{
    protected override void HandleInputs()
    {
        // left gyro
        leftGyro.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        leftGyro.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;

        // right gyro
        rightGyro.y = Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        rightGyro.x = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
    }
}