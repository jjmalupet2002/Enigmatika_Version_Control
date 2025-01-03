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

    // Reference to the NoteInspectionManager to check UI state
    private NoteInspectionManager noteInspectionManager;

    void Start()
    {
        // Add a listener to the zoom button to toggle the magnifying glass
        zoomButton.onClick.AddListener(ToggleMagnifyingGlass);

        // Find the NoteInspectionManager instance
        noteInspectionManager = NoteInspectionManager.Instance;
    }

    void Update()
    {
        // Check if the note UI is active and the magnifying glass is active
        if (noteInspectionManager != null && !noteInspectionManager.isNoteUIActive && isMagnifyingGlassActive)
        {
            // Disable the magnifying glass if note UI is not active
            ToggleMagnifyingGlass();
        }

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
        // Toggle the state of the magnifying glass
        isMagnifyingGlassActive = !isMagnifyingGlassActive;

        // Show or hide the magnifying glass
        magnifyingGlassRect.gameObject.SetActive(isMagnifyingGlassActive);

        // If note UI is inactive, ensure magnifying glass is hidden
        if (noteInspectionManager != null && !noteInspectionManager.isNoteUIActive)
        {
            isMagnifyingGlassActive = false; // Reset the magnifying glass state
            magnifyingGlassRect.gameObject.SetActive(false); // Hide it
        }
    }
}
