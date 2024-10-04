using UnityEngine;
using UnityEngine.UI;

public class NoteUIController : MonoBehaviour
{
    public static NoteUIController Instance { get; private set; }

    [Header("UI References")]
    public Button readButton; // Reference to the read button UI
    public CanvasGroup bookCanvasGroup; // Reference to the CanvasGroup for book-related UI

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
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

        // Ensure the CanvasGroup is initially disabled in the Inspector
        if (bookCanvasGroup != null)
        {
            bookCanvasGroup.gameObject.SetActive(false); // Disable the entire GameObject initially
        }
    }

    // Method to be called when the read button is pressed
    public void OnReadButtonPressed(NoteObjectHandler noteObject)
    {
        NoteInspectionManager.Instance.ToggleNoteUI(noteObject);
        ToggleReadButton(NoteInspectionManager.Instance.isNoteUIActive);
        ToggleBookCanvasGroup();
    }

    // Method to toggle the read button based on note inspection mode
    public void ToggleReadButton(bool isActive)
    {
        if (readButton != null)
        {
            readButton.gameObject.SetActive(isActive);
        }
    }

    // Method to toggle the GameObject for the book UI
    public void ToggleBookCanvasGroup()
    {
        if (bookCanvasGroup != null)
        {
            bool isActive = bookCanvasGroup.gameObject.activeSelf; // Check if the GameObject is active
            bookCanvasGroup.gameObject.SetActive(!isActive); // Toggle the active state of the GameObject

            // Optionally update interactable and raycast settings based on visibility
            bookCanvasGroup.interactable = !isActive;
            bookCanvasGroup.blocksRaycasts = !isActive;
        }
    }
}
