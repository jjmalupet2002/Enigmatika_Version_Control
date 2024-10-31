using UnityEngine;
using UnityEngine.UI; // Make sure to include this for Button

public class TestMovement : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Zoom button reference (toggling the magnifying glass object)")]
    public Button zoomButton;
    [Tooltip("Reference to the magnifying glass RectTransform")]
    public RectTransform magnifyingGlassRect;

    private bool isMagnifyingGlassActive = false; // Track the state of the magnifying glass
    private bool isDragging = false; // Track if the magnifying glass is currently being dragged

    void Start()
    {
        // Add a listener to the zoom button to toggle the magnifying glass
        zoomButton.onClick.AddListener(ToggleMagnifyingGlass);
    }

    void Update()
    {
        if (isMagnifyingGlassActive)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
            {
                // Check if the mouse is over the magnifying glass
                if (RectTransformUtility.RectangleContainsScreenPoint(magnifyingGlassRect, Input.mousePosition))
                {
                    isDragging = true; // Start dragging
                }
            }

            if (Input.GetMouseButtonUp(0)) // Left mouse button released
            {
                isDragging = false; // Stop dragging
            }

            if (isDragging)
            {
                // Update the position of the magnifying glass to follow the mouse cursor
                Vector3 mousePosition = Input.mousePosition;
                magnifyingGlassRect.position = mousePosition; // Move with mouse
            }
        }
    }

    public void ToggleMagnifyingGlass()
    {
        // Toggle the state
        isMagnifyingGlassActive = !isMagnifyingGlassActive;

        // Show or hide the magnifying glass
        magnifyingGlassRect.gameObject.SetActive(isMagnifyingGlassActive);
    }
}

