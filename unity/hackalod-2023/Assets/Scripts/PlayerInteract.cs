using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 2f;

    private Camera originCamera;

    private int interactableLayerMask;

    void Start() {
        originCamera = Camera.main;
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }

    internal void OnInteract(InputValue value) {
        if(value.Get<float>() > 0) {
            if (Physics.Raycast(
                originCamera.transform.position,
                originCamera.transform.forward,
                out RaycastHit hit,
                interactDistance,
                interactableLayerMask)
            ) {
                if (hit.collider.gameObject.TryGetComponent<Interactable>(out var interactable)) {
                    Debug.Log("Interacting with " + hit.collider.gameObject.name);
                    interactable.Interact();
                }
            }
        }
    }
}
