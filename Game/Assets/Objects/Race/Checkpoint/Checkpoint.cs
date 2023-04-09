using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Material activeMaterial;
    private Material inactiveMaterial;

    private Renderer render;
    private void Awake()
    {
        render = GetComponent<Renderer>();
        inactiveMaterial= render.material;
    }
    public void OnBecameTarget()
    {
        render.material = activeMaterial;
    }

    public void OnLostTarget()
    {
        render.material = inactiveMaterial;
    }
}
