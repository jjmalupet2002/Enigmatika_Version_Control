using UnityEngine;
using UnityEngine.UI;

public class HintUIManager : MonoBehaviour
{
    public Text hintText;
    public Text hintTitle;  // Optional
    public Text hintPointText;  // Text to display the hint point message
    public HintPointManager hintPointManager;
    public Button hintButton;  // The hint button
    private Hint currentHint;

    private void OnEnable()
    {
        HintEventManager.OnDisplayHint.AddListener(OnDisplayHint);
    }

    private void OnDisable()
    {
        HintEventManager.OnDisplayHint.RemoveListener(OnDisplayHint);
    }

    private void Start()
    {
        hintButton.onClick.AddListener(OnHintButtonPressed);
        hintButton.gameObject.SetActive(false);  // Hide button initially

        // Initial update of hint points UI
        UpdateHintPointsUI();
    }

    private void OnDisplayHint(Hint hint)
    {
        currentHint = hint;
        hintButton.gameObject.SetActive(true);  // Show the hint button
    }

    private void OnHintButtonPressed()
    {
        if (currentHint != null)
        {
            // Log currentHint.hintPoints to verify that it has the expected value
            Debug.Log("Attempting to subtract hint points: " + currentHint.hintPoints);

            // Check if there are enough hint points before subtracting
            if (hintPointManager.totalHintPoints >= currentHint.hintPoints)
            {
                HintEventManager.OnSubtractHintPoints.Invoke(currentHint.hintPoints);  // Subtract points
                ShowCurrentHint();
                hintButton.gameObject.SetActive(false);  // Hide button after using hint
                currentHint = null;  // Reset current hint after displaying
            }
            else
            {
                hintText.text = "Not enough hint points.";
            }
        }
    }

    public void ShowCurrentHint()
    {
        if (currentHint != null)
        {
            hintText.text = currentHint.hintText;
            hintTitle.text = currentHint.hintTitle;  // Optional
        }
    }

    public void ShowHintPointMessage(string message)
    {
        Debug.Log("Showing hint point message: " + message);  // Add a log to verify the message is passed
        hintPointText.text = message;
        hintPointText.gameObject.SetActive(true);
        Invoke("HideHintPointMessage", 3f);  // Hide message after 3 seconds
    }

    private void HideHintPointMessage()
    {
        hintPointText.gameObject.SetActive(false);
    }

    // This is the method you need to add to update the hint points UI
    private void UpdateHintPointsUI()
    {
        // Update the hint points UI here
        if (hintPointManager != null && hintPointText != null)
        {
            hintPointText.text = "Hint Points: " + hintPointManager.totalHintPoints;
        }
        Debug.Log("Updated hint points: " + hintPointManager.totalHintPoints);
    }
}
