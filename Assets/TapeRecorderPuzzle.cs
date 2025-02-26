using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using System.Diagnostics;

public class TapeRecorderPuzzle : MonoBehaviour
{
    [Header("Cassette UI Elements")]
    public TextMeshProUGUI text1; // Event 1 text display
    public TextMeshProUGUI text2; // Event 2 text display
    public GameObject textBackground1; // Parent of text1 (Enable/Disable)
    public GameObject textBackground2; // Parent of text2 (Enable/Disable)

    [Header("Cassette Reels & Controls")]
    public Transform cassette1; // Left reel (Event 1)
    public Transform cassette2; // Right reel (Event 2)
    public float rotationStep = 30f; // Rotation angle per step
    public float swipeThreshold = 50f; // Minimum swipe distance to register
    public float backgroundFadeDelay = 2f; // Time before UI disappears after no interaction

    [Header("Physical Confirm Button")]
    public Transform confirmButton; // Physical button object
    public Animator buttonAnimator; // Animator controller for button

    [Header("Event Data")]
    public List<EventData> cassette1Events = new List<EventData>();
    public List<EventData> cassette2Events = new List<EventData>();
    public Dictionary<int, int> correctPairs = new Dictionary<int, int>();

    private int cassette1Index = 0;
    private int cassette2Index = 0;
    private Vector2 startTouchPosition;
    private Transform selectedCassette = null;
    private Coroutine hideUIRoutine;

    private void Start()
    {
        if (cassette1Events.Count == 0 || cassette2Events.Count == 0)
        {
            UnityEngine.Debug.LogWarning("Cassette event lists are empty! Please assign values in the Inspector.");
        }

        // Hide UI at the start
        textBackground1.SetActive(false);
        textBackground2.SetActive(false);

        UpdateTextDisplay();
    }

    private void UpdateTextDisplay()
    {
        if (cassette1Events.Count > 0)
            text1.text = cassette1Events[cassette1Index].text;

        if (cassette2Events.Count > 0)
            text2.text = cassette2Events[cassette2Index].text;
    }

    public void PressConfirmButton()
    {
        // Play press animation
        buttonAnimator.SetTrigger("ButtonPress");

        // Check sequence after animation plays
        CheckSequence();

        // Reset button to idle after short delay
        Invoke("ResetButtonAnimation", 0.5f);
    }

    private void ResetButtonAnimation()
    {
        buttonAnimator.SetTrigger("ButtonIdle");
    }

    private void CheckSequence()
    {
        float cassette1X = cassette1.localEulerAngles.x;
        float cassette2X = cassette2.localEulerAngles.x;

        bool isCorrectMatch = Mathf.Approximately(cassette1X, cassette1Events[cassette1Index].rotationValues.x) &&
                              Mathf.Approximately(cassette2X, cassette2Events[cassette2Index].rotationValues.x) &&
                              correctPairs.ContainsKey(cassette1Index) && correctPairs[cassette1Index] == cassette2Index;

        if (isCorrectMatch)
        {
            UnityEngine.Debug.Log("Correct! Tape plays a clear sound.");
            PlayCorrectSequence();
        }
        else
        {
            UnityEngine.Debug.Log("Incorrect! Tape distorts.");
            PlayIncorrectSequence();
        }
    }

    private void PlayCorrectSequence()
    {
        textBackground1.GetComponent<Image>().color = Color.green;
        textBackground2.GetComponent<Image>().color = Color.green;
        // TODO: Play correct tape sound & reveal next clue
    }

    private void PlayIncorrectSequence()
    {
        textBackground1.GetComponent<Image>().color = Color.red;
        textBackground2.GetComponent<Image>().color = Color.red;
        Invoke("ResetBackgrounds", 0.5f);
        // TODO: Play distortion sound & shake UI
    }

    private void ResetBackgrounds()
    {
        textBackground1.GetComponent<Image>().color = Color.white;
        textBackground2.GetComponent<Image>().color = Color.white;
    }

    // Touch Input Handling (Using New Input System)
    public void OnSpinGesture(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Started)
        {
            startTouchPosition = touchPosition;
            selectedCassette = DetectCassetteUnderTouch(touchPosition);
        }
        else if (context.phase == InputActionPhase.Performed && selectedCassette != null)
        {
            float swipeDistance = touchPosition.x - startTouchPosition.x;

            if (Mathf.Abs(swipeDistance) > swipeThreshold)
            {
                bool isRightSwipe = swipeDistance > 0;

                if (selectedCassette == cassette1)
                {
                    RotateCassette1(isRightSwipe);
                    ShowUI(textBackground1); // Show UI for cassette 1
                }
                else if (selectedCassette == cassette2)
                {
                    RotateCassette2(isRightSwipe);
                    ShowUI(textBackground2); // Show UI for cassette 2
                }

                selectedCassette = null; // Reset selection after swipe
            }
        }
    }

    // Detect which cassette reel is being touched using Raycast
    private Transform DetectCassetteUnderTouch(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == cassette1)
                return cassette1;
            else if (hit.transform == cassette2)
                return cassette2;
            else if (hit.transform == confirmButton) // Detect if confirm button is pressed
            {
                PressConfirmButton();
                return null;
            }
        }

        return null;
    }

    public void RotateCassette1(bool clockwise)
    {
        cassette1Index = (cassette1Index + (clockwise ? 1 : -1) + cassette1Events.Count) % cassette1Events.Count;
        float targetX = cassette1Events[cassette1Index].rotationValues.x;
        cassette1.localRotation = Quaternion.Euler(targetX, 0, 0);
        UpdateTextDisplay();
    }

    public void RotateCassette2(bool clockwise)
    {
        cassette2Index = (cassette2Index + (clockwise ? 1 : -1) + cassette2Events.Count) % cassette2Events.Count;
        float targetX = cassette2Events[cassette2Index].rotationValues.x;
        cassette2.localRotation = Quaternion.Euler(targetX, 0, 0);
        UpdateTextDisplay();
    }

    private void ShowUI(GameObject textBackground)
    {
        textBackground.SetActive(true); // Enable UI

        // Cancel any existing hide coroutine
        if (hideUIRoutine != null)
        {
            StopCoroutine(hideUIRoutine);
        }

        // Start a coroutine to hide UI after a delay
        hideUIRoutine = StartCoroutine(HideUIAfterDelay());
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(backgroundFadeDelay);
        textBackground1.SetActive(false);
        textBackground2.SetActive(false);
    }

    [System.Serializable]
    public class EventData
    {
        public int index;
        public string text;
        public Vector3 rotationValues; // Now only the X rotation matters.
    }
}
