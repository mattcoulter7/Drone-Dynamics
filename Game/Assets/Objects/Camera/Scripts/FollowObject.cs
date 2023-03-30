using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SpeedScalar
{
    public float maxSpeed = 100f; // to calculate the scalar between 0 and 1, we need a max speed
    public float inValue = 0f; // value between 0 and 1 where the velocity starts taking effect
    public float outValue = 1f; // value between 0 and 1 where the velocity stops taking affect

    public Rigidbody rb;

    public float Apply(float v)
    {
        float speed = rb.velocity.magnitude;
        float speedScalar = speed / maxSpeed;

        // scale the scalar
        speedScalar -= inValue;
        speedScalar /= outValue;

        // ensure it is between 0 and 1
        speedScalar = Mathf.Clamp(speedScalar, 0.0f, 1.0f);

        return v * speedScalar;
    }
}

[System.Serializable]
public class MinMaxValue
{
    public float min;
    public float max;

    public float Apply(float v)
    {
        return Mathf.Clamp(v, min, max);
    }
}


[RequireComponent(typeof(Camera))]
public class FollowObject : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Height")]
    public float height = 2.0f;

    [Header("Speed")]
    public float velocityFactor = 0.2f;
    public float maxSpeed = 100.0f;

    [Header("Look Ahead")]
    public float lookAhead = 0.0f;
    public SpeedScalar lookAheadScalar;
    public MinMaxValue lookAheadRange;
    public float lookAheadSmoothing = 0.1f;

    [Header("Distance")]
    public float distance = 5.0f;
    public SpeedScalar distanceScalar;
    public MinMaxValue distanceRange;
    public float distanceSmoothing = 0.05f;

    [Header("FOV")]
    public float FOV = 60.0f;
    public SpeedScalar fovScalar;
    public MinMaxValue fovRange;
    public float fovSmoothing = 0.1f;

    [Header("Camera Instability")]
    public Vector3 instability = new Vector3(0.0f, 0.0f, 0.0f);
    public SpeedScalar instabilityScalar;
    public MinMaxValue instabilityRange;
    public float instabilitySmoothing = 0.05f;

    private Camera cam;
    private Transform originalTarget;

    private void Awake()
    {
        originalTarget = target;
        cam = GetComponent<Camera>();
        lookAheadScalar.rb = target.GetComponent<Rigidbody>();
        distanceScalar.rb = target.GetComponent<Rigidbody>();
        fovScalar.rb = target.GetComponent<Rigidbody>();
        instabilityScalar.rb = target.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null) return;

        InputAction spectate = playerInput.actions["spectate"];

        // Register callbacks for when the action is started (Down) and released (Up)
        spectate.started += (cc) =>
        {
            SyncComponent gmo = FindObjectsOfType<SyncComponent>().First(sc => !sc.local);
            if (gmo == null) return;

            target = gmo.transform;
        };
        spectate.canceled += (cc) =>
        {
            target = originalTarget;
        };
    }

    private void Update()
    {
        // Calculate the new distance based on velocity magnitude
        float desiredDistance = distanceRange.Apply(distanceRange.min + distanceScalar.Apply(
            distanceRange.max - distanceRange.min
        ));
        distance = Mathf.Lerp(distance, desiredDistance, distanceSmoothing);

        // Calculate the fov
        float desiredFOV = fovRange.Apply(fovRange.min + fovScalar.Apply(
            fovRange.max-fovRange.min
        ));
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, fovSmoothing);

        // Calculate the lookahead
        float desiredLookAhead = lookAheadRange.Apply(lookAheadRange.min + lookAheadScalar.Apply(
            lookAheadRange.max-lookAheadRange.min
        ));
        lookAhead = Mathf.Lerp(lookAhead, desiredLookAhead, lookAheadSmoothing);

        // Calculate the new position
        Vector3 desiredPosition = target.position - (target.forward * distance) + (Vector3.up * height);
        transform.position = desiredPosition;

        // Calculate the new rotation
        Quaternion desiredRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        transform.rotation = desiredRotation;

        // Calculate the new Look Angle
        float desiredInstabilityMagnitude = instabilityRange.Apply(instabilityRange.min + instabilityScalar.Apply(
            instabilityRange.max - instabilityRange.min
        ));
        Vector3 desiredInstability = new Vector3(
            Random.Range(-1, 1),
            Random.Range(-1, 1),
            Random.Range(-1, 1)
        ).normalized * desiredInstabilityMagnitude;
        instability = Vector3.Lerp(instability, desiredInstability, instabilitySmoothing);

        Vector3 aheadVector = new Vector3(target.forward.x, 0, target.forward.z);

        transform.LookAt(target.position + aheadVector * lookAhead + instability);
    }
}