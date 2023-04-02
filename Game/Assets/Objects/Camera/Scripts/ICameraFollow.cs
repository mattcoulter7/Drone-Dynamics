using UnityEngine;

public interface ICameraFollow
{
    Transform target { get; set; }
    void ReturnToOriginalTarget();
}