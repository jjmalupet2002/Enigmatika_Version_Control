using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimonSaysGrid : MonoBehaviour
{
    [System.Serializable]
    public class TileRow
    {
        public GameObject[] tiles = new GameObject[3]; // Assumes 3 columns
    }

    [Header("Grid Settings")]
    public TileRow[] grid = new TileRow[3]; // Assumes 3 rows

    [Header("Game Settings")]
    public float intervalTime = 3f;
    public Text safeTileText;

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
            yield return new WaitForSeconds(intervalTime);

            int safeRow = Random.Range(0, rows);
            int safeCol = Random.Range(0, cols);

            // Update UI
            if (safeTileText != null)
                safeTileText.text = $"Safe Tile: ({safeCol + 1}, {safeRow + 1})";

            // Activate only the safe tile
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r].tiles[c] != null)
                        grid[r].tiles[c].SetActive(r == safeRow && c == safeCol);
                }
            }

            // Wait again before resetting the grid
            yield return new WaitForSeconds(intervalTime);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r].tiles[c] != null)
                        grid[r].tiles[c].SetActive(true);
                }
            }
        }
    }
}