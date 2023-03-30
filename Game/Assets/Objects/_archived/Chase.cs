using UnityEngine;

public class Chase : MonoBehaviour
{
    public Transform target;
    public float speed = 10.0f;
    public float distance = 10.0f;
    public float height = 5.0f;

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component and disable gravity
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = target.position.y + height;
        transform.position = newPosition;
        // Look at the target
        transform.LookAt(target);

        // Calculate the target position
        Vector3 targetPosition = target.position - target.forward * distance + Vector3.up * height;

        // Move the camera towards the target using physics
        Vector3 velocity = (targetPosition - transform.position).normalized * speed;
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
        
    }
}