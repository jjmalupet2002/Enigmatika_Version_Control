using UnityEngine;
using UnityEngine.InputSystem; // Keep this line for Input System usage
using System.Collections.Generic;
using System.Collections;

public class NoteInspectionManager : MonoBehaviour
{
    public static NoteInspectionManager Instance { get; private set; }

    public Dictionary<NoteObjectHandler, List<GameObject>> noteUIs = new Dictionary<NoteObjectHandler, List<GameObject>>();
    private SwitchCamera switchCamera; // Reference to the SwitchCamera script
    private bool canInspectNotes; // Track if notes can be inspected
    public bool isNoteUIActive; // Track if the note UI is currently active

    public NoteObjectHandler currentNoteObject; // To keep track of the current note being viewed
    private int currentPageIndex; // To keep track of the current page being viewed

    private InputAction swipeLeftAction;
    private InputAction swipeRightAction;

    [Header("Input Action Asset Reference")]
    public InputActionAsset inputActionAsset; // Reference to your Input Action Asset

    private Vector2 swipeStartPos; // Start position of the swipe
    private float swipeStartTime; // Start time of the swipe
    private float swipeDuration = 0.3f; // Minimum duration for a valid swipe
    private float minSwipeDistance = 100f; // Minimum distance for a valid swipe

    // New variables for sound
    [Header("Sound Settings")]
    public AudioClip noteInteractSound; // Sound to play when interacting with a note
    public AudioClip bookInteractSound; // Sound to play when interacting with a book
    private AudioSource audioSource; // Reference to the AudioSource component

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

        // Initialize swipe actions
        swipeLeftAction = inputActionAsset.FindAction("SwipeLeft");
        swipeRightAction = inputActionAsset.FindAction("SwipeRight");

        // Subscribe to swipe actions
        swipeLeftAction.started += OnSwipeStart; // Capture swipe start
        swipeLeftAction.performed += OnSwipeLeft; // Directly pass the method
        swipeRightAction.started += OnSwipeStart; // Capture swipe start
        swipeRightAction.performed += OnSwipeRight; // Directly pass the method

        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component if not present
        audioSource.clip = noteInteractSound; // Assign the default AudioClip to the AudioSource
    }

    private void OnEnable()
    {
        swipeLeftAction.Enable();
        swipeRightAction.Enable();
    }

    private void OnDisable()
    {
        swipeLeftAction.Disable();
        swipeRightAction.Disable();
    }

    private void Update()
    {
        // Only allow note inspection if canInspectNotes is true and UI is not active
        if (canInspectNotes && !isNoteUIActive)
        {
            // Check for touch input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began) // Use UnityEngine.TouchPhase
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
    public void RegisterNoteUI(NoteObjectHandler noteObject, List<GameObject> notePages)
    {
        if (!noteUIs.ContainsKey(noteObject))
        {
            noteUIs[noteObject] = notePages;
            foreach (var page in notePages)
            {
                page.SetActive(false); // Hide all pages at the start
            }
        }
    }

    // Change method to public
    public void ToggleNoteUI(NoteObjectHandler noteObject)
    {
        // Check if the note object is tagged as "Book" for specific audio
        if (noteObject.CompareTag("Book"))
        {
            audioSource.clip = bookInteractSound; // Assign book interaction sound
        }
        else
        {
            audioSource.clip = noteInteractSound; // Default note interaction sound
        }

        // Play the interaction sound
        audioSource.Play(); // Play the sound when interacting with a note

        // Check if the note object exists in the dictionary
        if (noteUIs.TryGetValue(noteObject, out List<GameObject> notePages))
        {
            if (isNoteUIActive)
            {
                // Hide the current page and reset the UI state
                notePages[currentPageIndex].SetActive(false);
                isNoteUIActive = false;
                currentNoteObject = null; // Reset the current note object

                // Disable the book UI if it was active
                if (noteObject.CompareTag("Book"))
                {
                    NoteUIController.Instance.ToggleBookCanvasGroup();
                }
            }
            else
            {
                // Show the first page and update the UI state
                currentPageIndex = 0;
                notePages[currentPageIndex].SetActive(true);
                isNoteUIActive = true;
                currentNoteObject = noteObject; // Set the current note object

                // Check if the note object has the "Book" tag for special handling
                if (noteObject.CompareTag("Book"))
                {
                    // Enable the book-related UI
                    NoteUIController.Instance.ToggleBookCanvasGroup();
                }
            }

            // Update the read button state
            NoteUIController.Instance.ToggleReadButton(isNoteUIActive);
        }
    }

    // Capture the start position and time of the swipe
    private void OnSwipeStart(InputAction.CallbackContext context)
    {
        swipeStartPos = context.ReadValue<Vector2>();
        swipeStartTime = Time.time;
    }

    // Handle swipe left
    private void OnSwipeLeft(InputAction.CallbackContext context)
    {
        HandleSwipe(context, -1); // -1 indicates left swipe
    }

    // Handle swipe right
    private void OnSwipeRight(InputAction.CallbackContext context)
    {
        HandleSwipe(context, 1); // 1 indicates right swipe
    }

    // Generic swipe handler
    private void HandleSwipe(InputAction.CallbackContext context, int direction)
    {
        // Calculate swipe duration
        float swipeTime = Time.time - swipeStartTime;

        // Calculate the swipe distance
        Vector2 swipeEndPos = context.ReadValue<Vector2>();
        float swipeDistance = (swipeEndPos - swipeStartPos).magnitude;

        // Check for valid swipe conditions
        if (swipeTime >= swipeDuration && swipeDistance >= minSwipeDistance)
        {
            // Determine swipe direction
            Vector2 swipeDirection = swipeEndPos - swipeStartPos;
            float horizontalSwipe = swipeDirection.x;

            // Only process if the UI is active and a note is being viewed
            if (isNoteUIActive && currentNoteObject != null && noteUIs.TryGetValue(currentNoteObject, out List<GameObject> notePages))
            {
                if (notePages.Count > 1) // Check if there are multiple pages
                {
                    if (horizontalSwipe < 0 && currentPageIndex > 0) // Swipe left
                    {
                        UnityEngine.Debug.Log("Swipe Left Detected");
                        StartCoroutine(ChangePageWithDelay(notePages, currentPageIndex - 1));
                    }
                    else if (horizontalSwipe > 0 && currentPageIndex < notePages.Count - 1) // Swipe right
                    {
                        UnityEngine.Debug.Log("Swipe Right Detected");
                        StartCoroutine(ChangePageWithDelay(notePages, currentPageIndex + 1));
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Swipe {(horizontalSwipe < 0 ? "left" : "right")} has no effect.");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Cannot swipe, this note has only one page.");
                }
            }
        }
    }

    // New coroutine method to change pages with a delay
    private IEnumerator ChangePageWithDelay(List<GameObject> notePages, int newPageIndex)
    {
        // Hide the current page
        notePages[currentPageIndex].SetActive(false);

        // Wait for a slight delay (e.g., 0.5 seconds)
        yield return new WaitForSeconds(0.5f);

        // Update the current page index
        currentPageIndex = newPageIndex;

        // Show the new page
        notePages[currentPageIndex].SetActive(true);
    }

    // Method to enable or disable note inspection
    public void EnableNoteInspection(bool enable)
    {
        canInspectNotes = enable;
    }
}
