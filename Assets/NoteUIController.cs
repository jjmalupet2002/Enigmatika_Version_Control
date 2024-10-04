using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class NoteUIController : MonoBehaviour
{
    public static NoteUIController Instance { get; private set; }

    [Header("UI References")]
    public Button readButton; // Reference to the read button UI

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

        // Ensure the read button is initially disabled
        if (readButton != null)
        {
            readButton.gameObject.SetActive(false);
        }
    }

    // Method to be called when the read button is pressed
    public void OnReadButtonPressed(NoteObjectHandler noteObject)
    {
        NoteInspectionManager.Instance.ToggleNoteUI(noteObject);
        ToggleReadButton(NoteInspectionManager.Instance.isNoteUIActive);
    }

    // Method to toggle the read button based on note inspection mode
    public void ToggleReadButton(bool isActive)
    {
        if (readButton != null)
        {
            readButton.gameObject.SetActive(isActive);
            UnityEngine.Debug.Log("Read button toggled: " + isActive);
        }
    }
}
