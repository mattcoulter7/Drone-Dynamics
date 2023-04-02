using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FPV : MonoBehaviour, ICameraFollow
{
    [field: SerializeField] public Transform target { get; set; }
    [field: SerializeField] public float fov { get; private set; }

    [field: SerializeField] private Transform originalTarget;
    private Camera cam;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        transform.position = target.position;
        transform.rotation = target.rotation;
    }
    public void ReturnToOriginalTarget()
    {
        target = originalTarget;
    }

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = fov;
    }
}
