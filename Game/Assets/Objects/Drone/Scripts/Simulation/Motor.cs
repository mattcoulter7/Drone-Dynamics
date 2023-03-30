using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Motor : MonoBehaviour
{
    [field: Header("RPM")]
    [field: SerializeField] public float targetRpm { get; private set; } = 0f;
    [field: SerializeField] public float rpm { get; private set; } = 0f;
    

    [field: SerializeField] private float maxRpm = 10000f;
    [field: SerializeField] private float motorResponsiveness = 0.4f;

    [field: Header("Force Multiplier")]
    [field: SerializeField] private float thrustMultiplier = 1f;
    [field: SerializeField] private float torqueMultiplier = 1f;

    [field: Header("Configuration")]
    public float pitchMultiplier  = 0f;
    public float yawMultiplier = 0f;
    public float rollMultiplier = 0f;

    [field: Header("Propeller")]
    [field: SerializeField] private Transform propeller;
    [field: SerializeField] private float propellerDiameter = 0.3f;
    [field: SerializeField] private float propellerPitch = 1.0f;

    private Rigidbody droneRigidbody;
    private float propellerArea;
    private float propellerCoefficient;
    private float airDensity = 1.2f; // Standard air density at sea level

    public void SetRPM(float rpm)
    {
        targetRpm = Mathf.Clamp(rpm, 0, maxRpm);
    }

    // Start is called before the first frame update
    void Awake()
    {
        droneRigidbody = GetComponentInParent<Rigidbody>();
        propellerArea = Mathf.PI * Mathf.Pow(propellerDiameter / 2, 2);
        propellerCoefficient = 0.7f; // Change this to adjust for angle of attack and other factors
    }

    // Update is called once per frame
    void Update()
    {
        rpm = Mathf.Lerp(rpm, targetRpm, motorResponsiveness);

        float rotationSpeed = rpm * Time.deltaTime;
        propeller.Rotate(Vector3.up * Mathf.Sign(yawMultiplier), rotationSpeed);
    }
    private void FixedUpdate()
    {
        if (maxRpm == 0f) return;
        if (droneRigidbody == null) return;

        // Add Movement Force
        droneRigidbody.AddForceAtPosition(CalculateForce(), propeller.position);

        // Add Torque Force (from propeller)
        Vector3 toPropeller = propeller.position - droneRigidbody.worldCenterOfMass;
        Vector3 torque = transform.up * toPropeller.magnitude * torqueMultiplier * rpm;
        torque *= Mathf.Sign(yawMultiplier);
        droneRigidbody.AddTorque(torque);
    }
    private Vector3 CalculateForce()
    {
        // Calculate the force based on RPM, propeller diameter, pitch, and other factors
        float forceMagnitude = (rpm * propellerArea * propellerCoefficient) / 100.0f;

        // Calculate the pitch speed of the propeller based on the pitch angle and RPM
        float pitchSpeed = (2 * Mathf.PI * rpm) / 60.0f * propellerPitch;

        // Calculate the amount of air moved by the propeller based on the pitch speed and other factors
        float thrust = pitchSpeed * propellerDiameter;

        // Adjust the force magnitude based on the amount of air moved by the propeller
        forceMagnitude *= thrust;

        // Calculate the force vector in the direction of the forward vector of the propeller
        Vector3 forceVector = propeller.up * forceMagnitude;

        // Adjust the force magnitude based on air density
        forceVector *= airDensity;

        return forceVector * thrustMultiplier;
    }
}
