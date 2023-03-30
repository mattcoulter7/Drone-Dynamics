using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MotorController))]
public abstract class FlightController : MonoBehaviour
{
    [field: SerializeField] protected Vector2 leftGyro;
    [field: SerializeField] protected Vector2 rightGyro;

    private MotorController motorController;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        motorController = GetComponent<MotorController>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleInputs();

        leftGyro.x = Mathf.Clamp(leftGyro.x, -1f, 1f);
        leftGyro.y = Mathf.Clamp(leftGyro.y, -1f, 1f);
        rightGyro.x = Mathf.Clamp(rightGyro.x, -1f, 1f);
        rightGyro.y = Mathf.Clamp(rightGyro.y, -1f, 1f);

        motorController.thottle = (leftGyro.y + 1) / 2;
        motorController.yaw = leftGyro.x;
        motorController.pitch = rightGyro.y;
        motorController.roll = rightGyro.x;
    }

    protected abstract void HandleInputs();
}
