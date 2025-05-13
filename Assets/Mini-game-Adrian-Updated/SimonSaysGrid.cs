using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimonSaysGrid : MonoBehaviour
{
    [System.Serializable]
    public class TileRow
    {
        public GameObject[] tiles = new GameObject[3];
    }

    [Header("Grid Settings")]
    public TileRow[] grid = new TileRow[3];

    [Header("Game Settings")]
    public float showTextDelay = 2f;
    public float postCountdownPause = 0.5f;
    public float hideDuration = 2f;

    [Header("UI")]
    public Text safeTileText;
    public Text countdownText;

    [Header("Player Freeze Settings")]
    [Tooltip("Drag the GameObject you want to disable during freeze (usually the movement root or player body).")]
    public GameObject movementObject;

    private int rows => grid.Length;
    private int cols => grid[0].tiles.Length;

    void Start()
    {
        StartCoroutine(SimonSaysLoop());
    }

    IEnumerator SimonSaysLoop()
    {
        while (true)
        {
            int safeRow = Random.Range(0, rows);
            int safeCol = Random.Range(0, cols);

            if (safeTileText != null)
                safeTileText.text = $"Safe Tile: ({safeCol + 1}, {safeRow + 1})";

            if (countdownText != null)
                StartCoroutine(CountdownTimer(showTextDelay));

            yield return new WaitForSeconds(showTextDelay);

            // ✅ Disable player movement object
            if (movementObject != null)
                movementObject.SetActive(false);

            yield return new WaitForSeconds(postCountdownPause);

            // Remove tiles except the safe one
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r].tiles[c] != null)
                        grid[r].tiles[c].SetActive(r == safeRow && c == safeCol);
                }
            }

            if (countdownText != null)
                countdownText.text = "";

            yield return new WaitForSeconds(hideDuration);

            // Reactivate all tiles
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r].tiles[c] != null)
                        grid[r].tiles[c].SetActive(true);
                }
            }

            // ✅ Re-enable player movement object
            if (movementObject != null)
                movementObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CountdownTimer(float timeLeft)
    {
        while (timeLeft > 0f)
        {
            if (countdownText != null)
                countdownText.text = $"{timeLeft:F1}s";

            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }

        if (countdownText != null)
            countdownText.text = "Stop!";
    }
}
