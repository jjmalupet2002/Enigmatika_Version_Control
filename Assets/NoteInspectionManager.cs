using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoteInspectionManager : MonoBehaviour
{
    public static NoteInspectionManager Instance { get; private set; }

    public Dictionary<NoteObjectHandler, GameObject> noteUIs = new Dictionary<NoteObjectHandler, GameObject>();

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
    }

    private void Update()
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

    // Register a UI for a note object
    public void RegisterNoteUI(NoteObjectHandler noteObject, GameObject noteUI)
    {
        if (!noteUIs.ContainsKey(noteObject))
        {
            noteUIs[noteObject] = noteUI;
            noteUI.SetActive(false); // Hide the UI at the start
        }
    }

    private void ToggleNoteUI(NoteObjectHandler noteObject)
    {
        if (noteUIs.TryGetValue(noteObject, out GameObject noteUI))
        {
            // Toggle the UI active state
            bool isActive = noteUI.activeSelf;
            noteUI.SetActive(!isActive);

            // No need to update the image since sprite reference is removed
        }
    }
}