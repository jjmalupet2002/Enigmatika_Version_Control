using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintPointManager : MonoBehaviour
{
    public int totalHintPoints;

    private void OnEnable()
    {
        // Adding listeners for the hint events
        HintEventManager.OnAddHintPoints.AddListener(AddHintPoints);
        HintEventManager.OnSubtractHintPoints.AddListener(SubtractHintPoints);
    }

    private void OnDisable()
    {
        // Removing listeners when the object is disabled
        HintEventManager.OnAddHintPoints.RemoveListener(AddHintPoints);
        HintEventManager.OnSubtractHintPoints.RemoveListener(SubtractHintPoints);
    }

    // Method to add hint points
    public void AddHintPoints(int points)
    {
        totalHintPoints += points;
        Debug.Log("Hint points added: " + points + ". Total hint points: " + totalHintPoints);
        // Update UI or any other relevant components here
    }

    // Method to subtract hint points (return type is now void)
    public void SubtractHintPoints(int points)
    {
        Debug.Log("Attempting to subtract: " + points + " points.");

        if (totalHintPoints >= points)
        {
            totalHintPoints -= points;
            Debug.Log("Hint points subtracted: " + points + ". Total hint points: " + totalHintPoints);

            // Call a UI update method here, if necessary
            UpdateHintPointsUI();
        }
        else
        {
            Debug.Log("Not enough hint points.");
        }
    }

    private void UpdateHintPointsUI()
    {
        // Update the hint points UI here if needed
        Debug.Log("Updated hint points: " + totalHintPoints);
    }
}
