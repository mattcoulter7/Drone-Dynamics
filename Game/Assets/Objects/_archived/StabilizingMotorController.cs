using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilizingMotorController : MotorController
{
    [field: SerializeField] private float stabilityFactor = 0.1f; // a value between 0 and 1

    protected override float CalculateMotorRPM(Motor motor)
    {
        float motorRPM = base.CalculateMotorRPM(motor);

        // Calculate RPM based on stabilizing factor
        motorRPM += (1 - stabilityFactor) * (
            -motor.targetRpm // counteract current RPM to stabilize the drone
            + (pitch * pitchOffset * Mathf.Sign(motor.pitchMultiplier))
            + (yaw * yawOffset * Mathf.Sign(motor.yawMultiplier))
            + (roll * rollOffset * Mathf.Sign(motor.rollMultiplier))
        );

        return motorRPM;
    }
}