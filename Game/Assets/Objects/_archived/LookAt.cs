using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target; // The object the camera should follow
    public float zoomSpeed = 2.0f; // The speed at which the camera zooms in/out
    public float minZoom = 5.0f; // The minimum zoom distance
    public float maxZoom = 15.0f; // The maximum zoom distance
    public float targetScreenRatio = 0.5f; // The target size of the object on screen

    private Camera cam; // The camera component
    private Vector3 offset; // The offset between the camera and target
    private float targetSize; // The target size of the object on screen, in world units

    void Start()
    {
        cam = GetComponent<Camera>();
        offset = transform.position - target.position;

        // Calculate the initial target size of the object on screen
        targetSize = target.localScale.x / (2.0f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.aspect);
    }

    void LateUpdate()
    {
        // Calculate the current distance between the camera and target
        float currentDistance = Vector3.Distance(transform.position, target.position);

        // Calculate the target size of the object on screen based on the current distance
        float currentSize = target.localScale.x / (2.0f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.aspect);
        targetSize = Mathf.Lerp(targetSize, currentSize, Time.deltaTime * zoomSpeed * currentDistance);

        // Clamp the target size to the minimum and maximum values
        targetSize = Mathf.Clamp(targetSize, minZoom, maxZoom);

        // Calculate the target position of the camera
        Vector3 targetPosition = target.position + offset.normalized * targetSize;

        // Set the camera position to the target position without smoothing
        transform.position = targetPosition;

        // Set the camera size to match the target size
        cam.orthographicSize = targetSize / cam.aspect * targetScreenRatio;
    }
}