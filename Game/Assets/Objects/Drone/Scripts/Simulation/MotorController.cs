using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour
{
    [field: Header("Speed")]
    [field: SerializeField] public float thottle = 0f;
    [field: SerializeField] protected float maxRPM = 1f;

    [field: Header("Pitch")]
    [field: SerializeField] public float pitch = 0f;
    [field: SerializeField] protected float pitchOffset = 100f;

    [field: Header("Yaw")]
    [field: SerializeField] public float yaw = 0f;
    [field: SerializeField] protected float yawOffset = 100f;

    [field: Header("Roll")]
    [field: SerializeField] public float roll = 0f;
    [field: SerializeField] protected float rollOffset = 100f;

    protected Motor[] motors;

    private void Awake()
    {
        motors = GetComponentsInChildren<Motor>();
    }
    private void Update()
    {
        foreach (Motor motor in motors)
        {
            motor.SetRPM(
                CalculateMotorRPM(motor)
            );
        }
    }

    /// Calculates RPM for a given motor based on the target speed, pitch, yaw, and roll, as well as the speed and direction multipliers
    protected virtual float CalculateMotorRPM(Motor motor)
    {
        float motorRPM = 0f;

        // Calculate RPM based on target speed and direction multiplier
        float speedRPM = thottle * maxRPM;
        motorRPM += speedRPM;

        // Calculate RPM based on target pitch and pitch direction multiplier
        float pitchRPM = pitch * pitchOffset * Mathf.Sign(motor.pitchMultiplier);
        motorRPM += pitchRPM;

        // Calculate RPM based on target yaw and yaw direction multiplier
        float yawRPM = yaw * yawOffset * 2 * Mathf.Sign(motor.yawMultiplier);
        if (Mathf.Sign(yawRPM) == -1)
            motorRPM += yawRPM;

        // Calculate RPM based on target roll and roll direction multiplier
        float rollRPM = roll * rollOffset * Mathf.Sign(motor.rollMultiplier);
        motorRPM += rollRPM;

        return motorRPM;
    }
}