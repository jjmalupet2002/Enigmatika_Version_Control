using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapeRecorderPuzzle : MonoBehaviour
{
    [Header("Cassette UI Elements")]
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public GameObject textBackground1;
    public GameObject textBackground2;

    [Header("Cassette Reels & Controls")]
    public GameObject cassette1Object;
    public GameObject cassette2Object;
    public Button rotateButton1;
    public Button rotateButton2;

    [Header("Confirm Button Settings")]
    public GameObject confirmButtonObject;
    private Animator confirmButtonAnimator;

    private Animator cassette1Animator;
    private Animator cassette2Animator;
    private int cassette1Index = 0;
    private int cassette2Index = 0;
    private bool wasCloseUpActive = false;
    private bool isConfirmButtonMoving = false;
    private bool confirmPressed = false;
    private bool puzzleSolved = false; // Track puzzle solved state

    [Header("Event Data")]
    public List<EventData> cassette1Events = new List<EventData>();
    public List<EventData> cassette2Events = new List<EventData>();

    [Header("Correct Answer")]
    public List<int> correctAnswer = new List<int> { 1, 2 };

    [Header("Camera Reference")]
    public SwitchCamera switchCamera;

    [Header("Audio Settings")]
    public AudioSource rotateSound;
    public AudioSource confirmSound;

    [Header("Hidden Note Settings")]
    public GameObject hiddenNoteObject; // Reference to the hidden note GameObject
    private Animator hiddenNoteAnimator; // Animator reference

    private void Start()
    {
        cassette1Animator = cassette1Object.GetComponent<Animator>();
        cassette2Animator = cassette2Object.GetComponent<Animator>();

        textBackground1.SetActive(false);
        textBackground2.SetActive(false);

        rotateButton1.onClick.AddListener(() => RotateCassette(1));
        rotateButton2.onClick.AddListener(() => RotateCassette(2));

        // Store the initial position of the confirm button
        confirmButtonAnimator = confirmButtonObject.GetComponent<Animator>();

        // Get the Animator component from the hidden note
        if (hiddenNoteObject != null)
        {
            hiddenNoteAnimator = hiddenNoteObject.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        UpdateButtonVisibility();
        DetectConfirmButtonPress();
        UpdateConfirmButtonState();
    }

    private void UpdateButtonVisibility()
    {
        // Only show rotate buttons if THIS specific switchCamera instance is in CloseUp mode
        bool isCloseUpActive = switchCamera != null && switchCamera.currentCameraState == CameraState.CloseUp;

        rotateButton1.gameObject.SetActive(isCloseUpActive && !puzzleSolved); // Disable buttons if puzzle is solved
        rotateButton2.gameObject.SetActive(isCloseUpActive && !puzzleSolved); // Disable buttons if puzzle is solved

        if (!isCloseUpActive && wasCloseUpActive)
        {
            textBackground1.SetActive(false);
            textBackground2.SetActive(false);
        }

        wasCloseUpActive = isCloseUpActive;
    }

    private void UpdateConfirmButtonState()
    {
        bool isTextVisible = textBackground1.activeSelf && textBackground2.activeSelf;
        confirmButtonObject.GetComponent<BoxCollider>().enabled = isTextVisible;
    }

    private bool IsCloseUpCameraActive()
    {
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true;
            }
        }
        return false;
    }

    private void RotateCassette(int cassetteNumber)
    {
        if (cassetteNumber == 1)
        {
            cassette1Index = (cassette1Index - 1 + cassette1Events.Count) % cassette1Events.Count;
            StartCoroutine(PlayCassetteAnimation(cassette1Animator, cassette1Index, cassette1Events, text1, textBackground1));
        }
        else if (cassetteNumber == 2)
        {
            cassette2Index = (cassette2Index - 1 + cassette2Events.Count) % cassette2Events.Count;
            StartCoroutine(PlayCassetteAnimation(cassette2Animator, cassette2Index, cassette2Events, text2, textBackground2));
        }

        if (rotateSound != null)
        {
            rotateSound.Play();
        }
    }

    private IEnumerator PlayCassetteAnimation(Animator animator, int index, List<EventData> events, TextMeshProUGUI text, GameObject textBackground)
    {
        animator.SetTrigger("Spin");
        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(1f);

        DisplayEvent(index, events, text, textBackground);
    }

    private void DisplayEvent(int index, List<EventData> events, TextMeshProUGUI text, GameObject textBackground)
    {
        if (index >= 0 && index < events.Count)
        {
            text.text = events[index].text;
            textBackground.SetActive(true);
        }
        else
        {
            text.text = "";
            textBackground.SetActive(false);
        }
    }

    private void DetectConfirmButtonPress()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == confirmButtonObject)
                {
                    UnityEngine.Debug.Log("Confirm button pressed!");
                    confirmButtonAnimator.SetTrigger("PressDown");
                    confirmPressed = true;
                    PressConfirmButton();
                    Invoke(nameof(ResetConfirmButton), 1.5f);

                    if (confirmSound != null)
                    {
                        confirmSound.Play();
                    }
                }
            }
        }
    }

    private void ResetConfirmButton()
    {
        confirmButtonAnimator.SetTrigger("PressUp");
    }

    private void PressConfirmButton()
    {
        if (cassette1Index == correctAnswer[0] && cassette2Index == correctAnswer[1])
        {
            UnityEngine.Debug.Log("Correct Pair! Puzzle unlocked.");
            puzzleSolved = true; // Puzzle solved, disable spin buttons
            PlayCorrectSequence();
            MoveHiddenNote(); // Call the new method when puzzle is solved
        }
        else
        {
            UnityEngine.Debug.Log("Incorrect Pair! Try again.");
            PlayIncorrectSequence();
        }
    }

    private void PlayCorrectSequence()
    {
        textBackground1.GetComponent<Image>().color = Color.green;
        textBackground2.GetComponent<Image>().color = Color.green;
        textBackground1.SetActive(false);
        textBackground2.SetActive(false);
    }

    private void PlayIncorrectSequence()
    {
        textBackground1.GetComponent<Image>().color = Color.red;
        textBackground2.GetComponent<Image>().color = Color.red;
        // Removed the Invoke to reset color to white.
    }

    private void ResetBackgrounds()
    {
        textBackground1.GetComponent<Image>().color = Color.white;
        textBackground2.GetComponent<Image>().color = Color.white;
    }

    private void MoveHiddenNote()
    {
        if (hiddenNoteAnimator != null)
        {
            hiddenNoteObject.SetActive(true); // Ensure the note is visible
            hiddenNoteAnimator.SetTrigger("releaseNote"); // Trigger animation
        }
    }

    [System.Serializable]
    public class EventData
    {
        public int index;
        public string text;
    }
}
