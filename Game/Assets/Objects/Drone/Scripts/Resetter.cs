using UnityEngine;
using UnityEngine.InputSystem;

public class Resetter : MonoBehaviour
{
    ObjectSnapshot originalPosition = null;

    private void Start()
    {
        originalPosition = new ObjectSnapshot(transform);
    }
    private void OnReset(InputAction.CallbackContext cc)
    {
        originalPosition.Load(transform);
    }
    private void OnEnable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            return;

        playerInput.actions["reset"].performed += OnReset;
    }
    private void OnDisable()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            return;

        playerInput.actions["reset"].performed -= OnReset;
    }

}
