using UnityEngine;
using UnityEngine.UI;

public class HintUIManager : MonoBehaviour
{
    public HintPointManager hintPointManager;
    public Text hintPointsText; // Use Text if you're not using TextMeshPro
    public Button hintButton;
    public GameObject hintPointIcon;

    private void OnEnable()
    {
        hintPointManager.onHintPointsUpdated.AddListener(UpdateHintPointsUI);
    }

    private void OnDisable()
    {
        hintPointManager.onHintPointsUpdated.RemoveListener(UpdateHintPointsUI);
        hintPointManager.onHintButtonDisplay.RemoveListener(DisplayHintButton);
    }

    private void Start()
    {
        hintButton.onClick.AddListener(OnHintButtonPressed);
        UpdateHintPointsUI();
    }

    private void Update()
    {
        // Continuously check if the close-up camera is active and update the button visibility
        if (!IsCloseUpCameraActive())
        {
            hintButton.gameObject.SetActive(false); // Disable hint button if close-up camera is not active
        }
    }

    private void UpdateHintPointsUI()
    {
        hintPointsText.text = hintPointManager.hintPointsSO.hintPoints.ToString();
    }

    public void DisplayHintButton()
    {
        // Check if the close-up camera is active and show the hint button if so
        if (IsCloseUpCameraActive())
        {
            hintButton.gameObject.SetActive(true); // Show the hint button when the close-up camera is active
        }
    }

    private void OnHintButtonPressed()
    {
        hintPointManager.SubtractHintPoints(1); // Adjust the number of points subtracted as needed
        hintButton.gameObject.SetActive(false); // Optionally hide the button after use
    }

    // Method to check if the close-up camera is active
    private bool IsCloseUpCameraActive()
    {
        // Get all instances of SwitchCamera
        var switchCameras = FindObjectsOfType<SwitchCamera>();

        // Check if any instance has the CloseUp camera active
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true; // Return true if any close-up camera is active
            }
        }

        return false; // No close-up camera is active
    }
}
