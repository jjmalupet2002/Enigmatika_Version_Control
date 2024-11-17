using System.Diagnostics;
using UnityEngine;

public class TestUnlockTrigger : MonoBehaviour
{
    public Animator keyAnimator; // Reference to the key's Animator
    public string unlockTriggerName = "UnlockTrigger"; // Name of the trigger in Animator
    public KeyCode interactKey = KeyCode.I; // Key to press to trigger the unlock (I key)

    void Update()
    {
        // Check if the specified key (I) is pressed
        if (Input.GetKeyDown(interactKey))
        {
            // Ensure we have a reference to the Animator
            if (keyAnimator != null)
            {
                // Trigger the Unlock animation
                keyAnimator.SetTrigger(unlockTriggerName);

                UnityEngine.Debug.Log("Unlock trigger activated!");
            }
            else
            {
                UnityEngine.Debug.LogError("Animator is not assigned!");
            }
        }
    }
}
