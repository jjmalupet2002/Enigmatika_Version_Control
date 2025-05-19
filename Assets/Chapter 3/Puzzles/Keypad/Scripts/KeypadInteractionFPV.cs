using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad
{
    public class KeypadInteractionFPV : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera; // Assign this in the Inspector

        private void Awake()
        {
            if (interactionCamera == null)
            {
                interactionCamera = Camera.main; // Fallback to MainCamera if not assigned
            }
        }
        private void Update()
        {
            if (interactionCamera == null || !interactionCamera.isActiveAndEnabled) return;

            var ray = interactionCamera.ScreenPointToRay(Input.mousePosition);

            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f); // Draw a debug ray

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out var hit))
                {
                    Debug.Log($"[KeypadInteraction] Hit Object: {hit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                    if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                    {
                        UnityEngine.Debug.Log($"[KeypadInteraction] Found KeypadButton on {hit.collider.gameObject.name}");
                        keypadButton.PressButton();
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Raycast hit nothing!");
                }
            }
        }


        public void SetCamera(Camera newCamera)
        {
            interactionCamera = newCamera;
        }
    }
}
