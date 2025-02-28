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
    public Button rotateButton1; // Button for rotating cassette 1
    public Button rotateButton2; // Button for rotating cassette 2
    public Button confirmButton; // Button to confirm answer

    private Transform cassette1;
    private Transform cassette2;
    private int cassette1Index = 0;
    private int cassette2Index = 0;
    private bool wasCloseUpActive = false; // Track previous camera state

    [Header("Defined Rotation Angles")]
    public List<float> cassette1Rotations = new List<float> { 0f, 90f, 180f, 270f };
    public List<float> cassette2Rotations = new List<float> { 0f, 100f, 180f, 250f };

    [Header("Event Data")]
    public List<EventData> cassette1Events = new List<EventData>();
    public List<EventData> cassette2Events = new List<EventData>();
    public Dictionary<float, float> correctPairs = new Dictionary<float, float>(); // Correct rotation pairs

    [Header("Camera Reference")]
    public SwitchCamera switchCamera; // Reference to SwitchCamera script

    private void Start()
    {
        cassette1 = cassette1Object.transform;
        cassette2 = cassette2Object.transform;

        textBackground1.SetActive(false);
        textBackground2.SetActive(false);

        // Add button listeners
        rotateButton1.onClick.AddListener(() => RotateCassette(1));
        rotateButton2.onClick.AddListener(() => RotateCassette(2));
        confirmButton.onClick.AddListener(PressConfirmButton);

        UpdateButtonVisibility(); // Ensure correct initial state
    }

    private void Update()
    {
        UpdateButtonVisibility(); // Continuously check if buttons should be visible
        TrackCassetteRotation();  // Ensure correct tracking of rotation
    }

    private void UpdateButtonVisibility()
    {
        bool isCloseUpActive = IsCloseUpCameraActive();

        // Show or hide rotate buttons based on camera state
        rotateButton1.gameObject.SetActive(isCloseUpActive);
        rotateButton2.gameObject.SetActive(isCloseUpActive);

        // Hide text backgrounds when exiting close-up mode
        if (!isCloseUpActive && wasCloseUpActive)
        {
            textBackground1.SetActive(false);
            textBackground2.SetActive(false);
        }

        wasCloseUpActive = isCloseUpActive;
    }

    private void TrackCassetteRotation()
    {
        // Continuously track and adjust cassette rotation
        float cassette1X = NormalizeAngle(cassette1.localEulerAngles.x);
        float cassette2X = NormalizeAngle(cassette2.localEulerAngles.x);

        cassette1Index = FindClosestRotationIndex(cassette1X, cassette1Rotations);
        cassette2Index = FindClosestRotationIndex(cassette2X, cassette2Rotations);
    }

    private void RotateCassette(int cassetteNumber)
    {
        if (cassetteNumber == 1)
        {
            cassette1Index = (cassette1Index + 1) % cassette1Rotations.Count;
            SetCassetteRotation(cassette1, cassette1Rotations[cassette1Index]);
        }
        else if (cassetteNumber == 2)
        {
            cassette2Index = (cassette2Index + 1) % cassette2Rotations.Count;
            SetCassetteRotation(cassette2, cassette2Rotations[cassette2Index]);
        }

        CheckMatchingRotation();
    }

    private void SetCassetteRotation(Transform cassette, float targetRotation)
    {
        // Explicitly set only X-axis rotation to prevent unintended Y/Z flips
        cassette.localRotation = Quaternion.AngleAxis(targetRotation, Vector3.right);
    }

    private void CheckMatchingRotation()
    {
        float currentRotation1 = cassette1Rotations[cassette1Index];
        float currentRotation2 = cassette2Rotations[cassette2Index];

        bool cassette1HasEvent = false;
        bool cassette2HasEvent = false;

        foreach (EventData eventData in cassette1Events)
        {
            if (Mathf.Approximately(currentRotation1, eventData.rotationValue))
            {
                text1.text = eventData.text;
                cassette1HasEvent = true;
                break;
            }
        }

        foreach (EventData eventData in cassette2Events)
        {
            if (Mathf.Approximately(currentRotation2, eventData.rotationValue))
            {
                text2.text = eventData.text;
                cassette2HasEvent = true;
                break;
            }
        }

        textBackground1.SetActive(cassette1HasEvent);
        textBackground2.SetActive(cassette2HasEvent);
    }

    private void PressConfirmButton()
    {
        float currentRotation1 = cassette1Rotations[cassette1Index];
        float currentRotation2 = cassette2Rotations[cassette2Index];

        if (correctPairs.ContainsKey(currentRotation1) && correctPairs[currentRotation1] == currentRotation2)
        {
            UnityEngine.Debug.Log("Correct Pair! Puzzle progresses.");
            PlayCorrectSequence();
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
    }

    private void PlayIncorrectSequence()
    {
        textBackground1.GetComponent<Image>().color = Color.red;
        textBackground2.GetComponent<Image>().color = Color.red;
        Invoke(nameof(ResetBackgrounds), 0.5f);
    }

    private void ResetBackgrounds()
    {
        textBackground1.GetComponent<Image>().color = Color.white;
        textBackground2.GetComponent<Image>().color = Color.white;
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

    private int FindClosestRotationIndex(float currentRotation, List<float> rotationList)
    {
        int closestIndex = 0;
        float closestDistance = Mathf.Abs(currentRotation - rotationList[0]);

        for (int i = 1; i < rotationList.Count; i++)
        {
            float distance = Mathf.Abs(currentRotation - rotationList[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return angle;
    }

    [System.Serializable]
    public class EventData
    {
        public int index;
        public string text;
        public float rotationValue;
    }
}
