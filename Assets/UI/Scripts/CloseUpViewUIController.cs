using System.Diagnostics;
using UnityEngine;

public class CloseUpViewUIController : MonoBehaviour
{
    // Reference to the UI GameObjects
    
    public GameObject newItemDiscoveredPanel;
    public GameObject hintIcon;

    // Method to enable or disable the UI elements
    public void SetUIActive(bool isActive)
    {
        UnityEngine.Debug.Log("SetUIActive called with isActive: " + isActive);

        // Enable or disable the GameObjects based on the isActive parameter
       
        newItemDiscoveredPanel.SetActive(isActive);
        hintIcon.SetActive(isActive);
    }

    // Optional: Call this method to disable the UI on exit
    public void DisableUI()
    {
        SetUIActive(false);
    }
}
