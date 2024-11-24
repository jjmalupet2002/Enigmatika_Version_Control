using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Hint hint;  // Reference to the Hint ScriptableObject

    // Call this method when the player fails the puzzle
    public void OnPuzzleFail()
    {
        // Trigger the hint event to display the hint button
        HintEventManager.OnDisplayHint.Invoke(hint);
    }

    // Call this method when the player completes the puzzle successfully
    public void OnPuzzleComplete()
    {
        // Call the event to add hint points
        HintEventManager.OnAddHintPoints.Invoke(1); // Adds 5 hint points
    }

    // Example trigger for puzzle fail, this can be replaced by your specific condition
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Simulating failure with the F key
        {
            OnPuzzleFail();
        }
        if (Input.GetKeyDown(KeyCode.C)) // Simulating completion with the C key
        {
            OnPuzzleComplete();
        }
    }
}
