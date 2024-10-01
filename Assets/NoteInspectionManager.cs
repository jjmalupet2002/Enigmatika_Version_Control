using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class NoteInspectionManager : MonoBehaviour
{
    public static NoteInspectionManager Instance { get; private set; }

    public Dictionary<NoteObjectHandler, GameObject> noteUIs = new Dictionary<NoteObjectHandler, GameObject>();
    private SwitchCamera switchCamera; // Reference to the SwitchCamera script
    private bool canInspectNotes; // Track if notes can be inspected
    public bool isNoteUIActive; // Track if the note UI is currently active

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if it already exists
            return;
        }

        Instance = this; // Set the singleton instance
        DontDestroyOnLoad(gameObject); // Optional: Keep it between scene loads

        // Find the SwitchCamera instance
        switchCamera = FindObjectOfType<SwitchCamera>();
        if (switchCamera == null)
        {
            UnityEngine.Debug.LogError("SwitchCamera script not found.");
        }
    }

    private void Update()
    {
        // Only allow note inspection if canInspectNotes is true and UI is not active
        if (canInspectNotes && !isNoteUIActive)
        {
            // Check for touch input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    NoteObjectHandler noteObject = hit.collider.GetComponent<NoteObjectHandler>();
                    if (noteObject != null)
                    {
                        ToggleNoteUI(noteObject);
                    }
                }
            }
        }
    }

    // Register a UI for a note object
    public void RegisterNoteUI(NoteObjectHandler noteObject, GameObject noteUI)
    {
        if (!noteUIs.ContainsKey(noteObject))
        {
            noteUIs[noteObject] = noteUI;
            noteUI.SetActive(false); // Hide the UI at the start
        }
    }

    // Change method to public
    public void ToggleNoteUI(NoteObjectHandler noteObject) // Make this public
    {
        if (noteUIs.TryGetValue(noteObject, out GameObject noteUI))
        {
            // Toggle the UI active state
            bool isActive = noteUI.activeSelf;
            noteUI.SetActive(!isActive);

            // Update the note UI active state flag
            if (isActive) // If we are closing the UI
            {
                isNoteUIActive = false; // Reset the flag when the UI is closed
            }
            else // If we are opening the UI
            {
                isNoteUIActive = true; // Set the flag when the UI is opened
            }
        }
    }

    // Method to enable or disable note inspection
    public void EnableNoteInspection(bool enable)
    {
        canInspectNotes = enable;
    }
}