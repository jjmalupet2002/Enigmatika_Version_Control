using System.Diagnostics;
using UnityEngine;

public class CloseUpViewUIController : MonoBehaviour
{
    // Reference to the Canvas Groups
    public CanvasGroup takeItemButton;
    public CanvasGroup newItemDiscoveredPanel;
    public CanvasGroup hintIcon;

    // Method to enable the UI elements
    public void SetUIActive(bool isActive)
    {
        UnityEngine.Debug.Log("SetUIActive called with isActive: " + isActive);

        // Enable or disable the GameObjects based on the isActive parameter
        takeItemButton.gameObject.SetActive(isActive);
        newItemDiscoveredPanel.gameObject.SetActive(isActive);
        hintIcon.gameObject.SetActive(isActive);
    }

    // Optional: Call this method to disable the UI on exit
    public void DisableUI()
    {
        SetUIActive(false);
    }
}
