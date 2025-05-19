using UnityEngine;

public class HintKeyControl : MonoBehaviour
{
    public HintPointManager hintPointManager;
    public KeyCode addPointsKey = KeyCode.A; // Default key to add points
    public KeyCode subtractPointsKey = KeyCode.S; // Default key to subtract points
    public int pointsToAdd = 1;
    public int pointsToSubtract = 1;

    void Update()
    {
        if (Input.GetKeyDown(addPointsKey))
        {
            hintPointManager.AddHintPoints(pointsToAdd);
        }

        if (Input.GetKeyDown(subtractPointsKey))
        {
            hintPointManager.DisplayHintButton(); // Display the hint button when subtracting points
        }
    }
}
