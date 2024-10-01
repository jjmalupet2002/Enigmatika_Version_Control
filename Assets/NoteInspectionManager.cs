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

    private NoteObjectHandler currentNoteObject; // To keep track of the current note being viewed
    private int currentPageIndex; // To keep track of the current page being viewed

    private InputAction swipeLeftAction;
    private InputAction swipeRightAction;

    [Header("Input Action Asset Reference")]
    public InputActionAsset inputActionAsset; // Reference to your Input Action Asset

    private Vector2 swipeStartPos; // Start position of the swipe
    private float swipeStartTime; // Start time of the swipe
    private float swipeDuration = 0.3f; // Minimum duration for a valid swipe
    private float minSwipeDistance = 100f; // Minimum distance for a valid swipe

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
            Debug.LogError("SwitchCamera script not found.");
        }

        // Initialize swipe actions
        swipeLeftAction = inputActionAsset.FindAction("SwipeLeft");
        swipeRightAction = inputActionAsset.FindAction("SwipeRight");

        // Subscribe to swipe actions
        swipeLeftAction.started += OnSwipeStart; // Capture swipe start
        swipeLeftAction.performed += OnSwipeLeft; // Directly pass the method
        swipeRightAction.started += OnSwipeStart; // Capture swipe start
        swipeRightAction.performed += OnSwipeRight; // Directly pass the method
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
    public void ToggleNoteUI(NoteObjectHandler noteObject) // Make this public
    {
        if (noteUIs.TryGetValue(noteObject, out List<GameObject> notePages))
        {
            if (isNoteUIActive)
            {
                // Hide the current page
                notePages[currentPageIndex].SetActive(false);
                isNoteUIActive = false;
                canInspectNotes = true; // Allow note inspection again
                currentNoteObject = null; // Reset the current note object
            }
            else
            {
                // Show the first page
                currentPageIndex = 0;
                notePages[currentPageIndex].SetActive(true);
                isNoteUIActive = true;
                canInspectNotes = false; // Disable note inspection
                currentNoteObject = noteObject; // Set the current note object
            }
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
                        Debug.Log("Swipe Left Detected");
                        StartCoroutine(ChangePageWithDelay(notePages, currentPageIndex - 1));
                    }
                    else if (horizontalSwipe > 0 && currentPageIndex < notePages.Count - 1) // Swipe right
                    {
                        Debug.Log("Swipe Right Detected");
                        StartCoroutine(ChangePageWithDelay(notePages, currentPageIndex + 1));
                    }
                    else
                    {
                        Debug.Log($"Swipe {(horizontalSwipe < 0 ? "left" : "right")} has no effect.");
                    }
                }
                else
                {
                    Debug.Log("Cannot swipe, this note has only one page.");
                }
            }
        }
    }

    private IEnumerator ChangePageWithDelay(List<GameObject> notePages, int newPageIndex)
    {
        // Start fading out the current page
        yield return StartCoroutine(FadeOut(notePages[currentPageIndex]));

        // Update the current page index
        currentPageIndex = newPageIndex;

        // Start fading in the new page
        yield return StartCoroutine(FadeIn(notePages[currentPageIndex]));
    }

    // New coroutine for fading out a page
    private IEnumerator FadeOut(GameObject page)
    {
        CanvasGroup canvasGroup = page.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = page.AddComponent<CanvasGroup>(); // Add a CanvasGroup if one doesn't exist
        }

        // Fade out over 0.5 seconds
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsed / duration)); // Decrease alpha
            yield return null; // Wait for the next frame
        }

        // Ensure it's fully transparent
        canvasGroup.alpha = 0;
        page.SetActive(false); // Set the page inactive after fading out
    }

    // New coroutine for fading in a page
    private IEnumerator FadeIn(GameObject page)
    {
        CanvasGroup canvasGroup = page.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = page.AddComponent<CanvasGroup>(); // Add a CanvasGroup if one doesn't exist
        }

        page.SetActive(true); // Make the page active before fading in
        canvasGroup.alpha = 0; // Start fully transparent

        // Fade in over 0.5 seconds
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration); // Increase alpha
            yield return null; // Wait for the next frame
        }

        // Ensure it's fully opaque
        canvasGroup.alpha = 1;
    }


    // Method to enable or disable note inspection
    public void EnableNoteInspection(bool enable)
    {
        canInspectNotes = enable;
    }
}