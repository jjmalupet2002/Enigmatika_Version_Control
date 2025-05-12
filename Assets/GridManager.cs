using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject floorTilePrefab;
    public int rows = 3;
    public int cols = 3;
    public float spacing = 1.1f;

    [Header("Game Settings")]
    public float intervalTime = 3f;
    public Text safeTileText; // Assign from Unity UI

    private GameObject[,] tiles;

    void Start()
    {
        GenerateGrid();
        StartCoroutine(SimonSaysLoop());
    }

    void GenerateGrid()
    {
        tiles = new GameObject[rows, cols];
        Vector3 startPos = transform.position - new Vector3(cols / 2f, 0, rows / 2f);

        for (int x = 0; x < cols; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 pos = startPos + new Vector3(x * spacing, 0, z * spacing);
                GameObject tile = Instantiate(floorTilePrefab, pos, Quaternion.identity, transform);
                tiles[x, z] = tile;
            }
        }
    }

    IEnumerator SimonSaysLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalTime);

            int safeX = Random.Range(0, cols);
            int safeZ = Random.Range(0, rows);

            // Update UI text
            if (safeTileText != null)
                safeTileText.text = $"Safe Tile: ({safeX + 1}, {safeZ + 1})";

            // Enable only the safe tile
            for (int x = 0; x < cols; x++)
            {
                for (int z = 0; z < rows; z++)
                {
                    tiles[x, z].SetActive(x == safeX && z == safeZ);
                }
            }

            // Wait and then restore all tiles
            yield return new WaitForSeconds(intervalTime);

            for (int x = 0; x < cols; x++)
            {
                for (int z = 0; z < rows; z++)
                {
                    tiles[x, z].SetActive(true);
                }
            }
        }
    }
}
