using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimonSaysGrid : MonoBehaviour
{
    [System.Serializable]
    public class TileRow
    {
        public GameObject[] tiles = new GameObject[3];
    }

    [System.Serializable]
    public enum TileDescriptionType
    {
        Standard,       // Use row/column values directly
        Directional,    // Use directions (N/S/E/W or NW/NE/SW/SE)
        ColorBased,     // Describe based on custom colors you assign to tiles
        Custom          // Use entirely custom riddle
    }

    [System.Serializable]
    public class RiddleEntry
    {
        [Tooltip("Type of riddle description to use")]
        public TileDescriptionType descriptionType = TileDescriptionType.Standard;
        
        [Tooltip("Format: Use {row}, {col}, {dir}, {color}, {position}, {corner}, {edge}, {center}, {even_odd} as placeholders")]
        public string riddleText = "The safe tile is at row {row}, column {col}";
        
        [Tooltip("Optional: Custom condition when this riddle should be used")]
        public bool useOnlyForSpecificTiles = false;
        public int specificRow = -1;
        public int specificCol = -1;
    }

    [System.Serializable]
    public class TileColor
    {
        public string colorName = "Red";
        public int row = 0;
        public int col = 0;
    }

    [Header("Grid Settings")]
    public TileRow[] grid = new TileRow[3];

    [Header("Game Settings")]
    public float showTextDelay = 2f;
    public float postCountdownPause = 0.5f;
    public float hideDuration = 2f;

    [Header("Additional Objects")]
    [Tooltip("Additional objects to remove during the freeze period")]
    public GameObject[] additionalObjects;
    [Tooltip("Should additional objects disappear with normal tiles?")]
    public bool hideAdditionalWithTiles = true;
    [Tooltip("Should additional objects fade out instead of instantly disappearing?")]
    public bool fadeOutAdditional = false;
    [Tooltip("If fading is enabled, how long should the fade take?")]
    public float fadeOutDuration = 0.5f;

    [Header("Riddle Settings")]
    [Tooltip("Add riddles/hints that will be used to reveal the safe tile")]
    public List<RiddleEntry> safePositionHints = new List<RiddleEntry>();
    [Tooltip("If true, will use a random hint from the list. If false, cycles through them in order.")]
    public bool useRandomHints = true;
    [Tooltip("Specify custom color names for specific tiles")]
    public List<TileColor> tileColors = new List<TileColor>();
    
    private int currentHintIndex = 0;

    [Header("UI")]
    public Text safeTileText;
    public Text countdownText;

    [Header("Player Freeze Settings")]
    [Tooltip("Drag the GameObject you want to disable during freeze (usually the movement root or player body).")]
    public GameObject movementObject;

    private int rows => grid.Length;
    private int cols => grid[0].tiles.Length;
    
    // Store original states of additional objects
    private Dictionary<GameObject, bool> originalObjectStates = new Dictionary<GameObject, bool>();
    // Store original renderers for fade effects
    private Dictionary<GameObject, Renderer[]> objectRenderers = new Dictionary<GameObject, Renderer[]>();

    void Awake()
    {
        // Store the initial state of additional objects
        if (additionalObjects != null)
        {
            foreach (var obj in additionalObjects)
            {
                if (obj != null)
                {
                    originalObjectStates[obj] = obj.activeSelf;
                    if (fadeOutAdditional)
                    {
                        objectRenderers[obj] = obj.GetComponentsInChildren<Renderer>();
                    }
                }
            }
        }
    }

    void Start()
    {
        // Initialize with default hints if none are provided
        if (safePositionHints.Count == 0)
        {
            // Standard description
            safePositionHints.Add(new RiddleEntry { 
                descriptionType = TileDescriptionType.Standard,
                riddleText = "The safe tile is at position ({col}, {row})" 
            });
            
            // Directional description
            safePositionHints.Add(new RiddleEntry { 
                descriptionType = TileDescriptionType.Directional,
                riddleText = "The safe tile is in the {dir} of the grid" 
            });
            
            // Position-based description
            safePositionHints.Add(new RiddleEntry { 
                descriptionType = TileDescriptionType.Custom,
                riddleText = "Find the safe tile at the {position}" 
            });
        }

        StartCoroutine(SimonSaysLoop());
    }

    IEnumerator SimonSaysLoop()
    {
        while (true)
        {
            // Select a random safe position using 0-based indices internally
            int safeRow = Random.Range(0, rows);
            int safeCol = Random.Range(0, cols);

            if (safeTileText != null)
            {
                // Pass the 0-based indices to GetNextHint
                string hintText = GetNextHint(safeRow, safeCol);
                safeTileText.text = hintText;
            }

            if (countdownText != null)
                StartCoroutine(CountdownTimer(showTextDelay));

            yield return new WaitForSeconds(showTextDelay);

            // Disable player movement object
            if (movementObject != null)
                movementObject.SetActive(false);

            yield return new WaitForSeconds(postCountdownPause);

            // Handle additional objects before removing tiles if configured that way
            if (hideAdditionalWithTiles && additionalObjects != null)
            {
                foreach (var obj in additionalObjects)
                {
                    if (obj != null)
                    {
                        if (fadeOutAdditional && objectRenderers.ContainsKey(obj))
                        {
                            StartCoroutine(FadeOutObject(obj, fadeOutDuration));
                        }
                        else
                        {
                            obj.SetActive(false);
                        }
                    }
                }
            }

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

            // Restore additional objects
            if (additionalObjects != null)
            {
                foreach (var obj in additionalObjects)
                {
                    if (obj != null)
                    {
                        // If we were fading, make sure all renderers are fully visible again
                        if (fadeOutAdditional && objectRenderers.ContainsKey(obj))
                        {
                            foreach (var renderer in objectRenderers[obj])
                            {
                                if (renderer != null)
                                {
                                    Color color = renderer.material.color;
                                    color.a = 1f;
                                    renderer.material.color = color;
                                }
                            }
                        }
                        // Set object active state back to its original state
                        obj.SetActive(originalObjectStates.ContainsKey(obj) ? originalObjectStates[obj] : true);
                    }
                }
            }

            // Re-enable player movement object
            if (movementObject != null)
                movementObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FadeOutObject(GameObject obj, float duration)
    {
        if (!objectRenderers.ContainsKey(obj))
            yield break;

        // Store original alpha values
        Dictionary<Renderer, float> originalAlpha = new Dictionary<Renderer, float>();
        foreach (var renderer in objectRenderers[obj])
        {
            if (renderer != null)
            {
                originalAlpha[renderer] = renderer.material.color.a;
            }
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            
            // Update alpha for all renderers
            foreach (var renderer in objectRenderers[obj])
            {
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = Mathf.Lerp(originalAlpha[renderer], 0f, normalizedTime);
                    renderer.material.color = color;
                }
            }
            
            yield return null;
        }
        
        // Fully hide the object at the end of fade
        obj.SetActive(false);
    }

    string GetNextHint(int safeRow, int safeCol)
    {
        if (safePositionHints.Count == 0)
            return $"Safe Tile: ({safeCol + 1}, {safeRow + 1})";

        // Display raw coordinates by default (1-based indices for user-friendly display)
        int displayRow = safeRow + 1;
        int displayCol = safeCol + 1;

        // First, filter for any specific tile riddles
        List<RiddleEntry> validRiddles = new List<RiddleEntry>();
        
        foreach (var hint in safePositionHints)
        {
            // Check if this riddle is restricted to a specific tile
            if (hint.useOnlyForSpecificTiles)
            {
                // Use 0-based comparison since we store 0-based indices internally
                if (hint.specificRow == safeRow && hint.specificCol == safeCol)
                {
                    validRiddles.Add(hint);
                }
            }
            else
            {
                // This riddle isn't tile-specific, so it's always valid
                validRiddles.Add(hint);
            }
        }

        // If no valid riddles found, fall back to default
        if (validRiddles.Count == 0)
            return $"Safe Tile: ({displayCol}, {displayRow})";

        // Initialize selectedHint to the first valid riddle to ensure it's always assigned
        RiddleEntry selectedHint = validRiddles[0];
        
        if (useRandomHints)
        {
            // Select a random hint from valid riddles
            int randomIndex = Random.Range(0, validRiddles.Count);
            selectedHint = validRiddles[randomIndex];
        }
        else
        {
            // Find the next valid riddle based on current index
            int startIndex = currentHintIndex;
            bool found = false;
            
            // Loop through hints starting from current index
            for (int i = 0; i < safePositionHints.Count && !found; i++)
            {
                int checkIndex = (startIndex + i) % safePositionHints.Count;
                
                // If this hint is in our valid list
                if (validRiddles.Contains(safePositionHints[checkIndex]))
                {
                    selectedHint = safePositionHints[checkIndex];
                    currentHintIndex = (checkIndex + 1) % safePositionHints.Count;
                    found = true;
                }
            }
            
            // If somehow no valid hints were found in the loop (should never happen due to our check above),
            // we've already initialized selectedHint to the first valid riddle
        }

        // Get the base hint text
        string hintText = selectedHint.riddleText;
        
        // Replace all possible placeholders based on the description type
        hintText = hintText.Replace("{row}", displayRow.ToString());
        hintText = hintText.Replace("{col}", displayCol.ToString());
        
        // Handle directional description - use 0-based indices for internal logic
        string direction = GetDirectionalDescription(safeRow, safeCol);
        hintText = hintText.Replace("{dir}", direction);
        
        // Handle color-based description
        string tileColor = GetTileColor(safeRow, safeCol);
        hintText = hintText.Replace("{color}", tileColor);
        
        // Handle position description (center, corner, edge)
        string positionDesc = GetPositionDescription(safeRow, safeCol);
        hintText = hintText.Replace("{position}", positionDesc);
        
        // Handle corner description (NW, NE, SW, SE)
        string cornerDesc = GetCornerDescription(safeRow, safeCol);
        hintText = hintText.Replace("{corner}", cornerDesc);
        
        // Handle edge description (N, S, E, W)
        string edgeDesc = GetEdgeDescription(safeRow, safeCol);
        hintText = hintText.Replace("{edge}", edgeDesc);
        
        // Handle center description
        string centerDesc = (safeRow == rows/2 && safeCol == cols/2) ? "center" : "not center";
        hintText = hintText.Replace("{center}", centerDesc);
        
        // Handle even/odd description
        string evenOddRow = (safeRow % 2 == 0) ? "even" : "odd";
        string evenOddCol = (safeCol % 2 == 0) ? "even" : "odd";
        hintText = hintText.Replace("{even_odd_row}", evenOddRow);
        hintText = hintText.Replace("{even_odd_col}", evenOddCol);

        return hintText;
    }
    
    string GetDirectionalDescription(int row, int col)
    {
        // For a 3x3 grid, determine general direction
        // We're using 0-based indices internally, so check for 0 and max values
        if (row == 0 && col == 0) return "northwest";
        if (row == 0 && col == cols-1) return "northeast";
        if (row == rows-1 && col == 0) return "southwest";
        if (row == rows-1 && col == cols-1) return "southeast";
        
        if (row == 0) return "north";
        if (row == rows-1) return "south";
        if (col == 0) return "west";
        if (col == cols-1) return "east";
        
        return "center";
    }
    
    string GetCornerDescription(int row, int col)
    {
        // We're using 0-based indices internally
        if (row == 0 && col == 0) return "northwest corner";
        if (row == 0 && col == cols-1) return "northeast corner";
        if (row == rows-1 && col == 0) return "southwest corner";
        if (row == rows-1 && col == cols-1) return "southeast corner";
        
        return "not a corner";
    }
    
    string GetEdgeDescription(int row, int col)
    {
        // We're using 0-based indices internally
        if (row == 0) return "north edge";
        if (row == rows-1) return "south edge";
        if (col == 0) return "west edge";
        if (col == cols-1) return "east edge";
        
        return "not an edge";
    }
    
    string GetPositionDescription(int row, int col)
    {
        // For a 3x3 grid using 0-based indices internally
        if (row == 1 && col == 1) return "center";
        
        if (row == 0 && col == 0) return "top left";
        if (row == 0 && col == 1) return "top middle";
        if (row == 0 && col == 2) return "top right";
        
        if (row == 1 && col == 0) return "middle left";
        if (row == 1 && col == 2) return "middle right";
        
        if (row == 2 && col == 0) return "bottom left";
        if (row == 2 && col == 1) return "bottom middle";
        if (row == 2 && col == 2) return "bottom right";
        
        // For larger grids, we need to use user-friendly 1-based indices
        return $"row {row+1}, column {col+1}";
    }
    
    string GetTileColor(int row, int col)
    {
        // Check if this tile has a custom color
        // We're using 0-based indices internally
        foreach (var tileColor in tileColors)
        {
            if (tileColor.row == row && tileColor.col == col)
            {
                return tileColor.colorName;
            }
        }
        
        // Default fallback
        return "unknown";
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