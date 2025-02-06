


using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;  // For UI elements


public class BoardManager : MonoBehaviour
{
    public Text shuffleMessage; // Reference to the UI text element

    public int rows = 10;
    public int columns = 10;
    public float cellSize = 2.56f; // Add this to the top of BoardManager.cs
    private CameraManager cameraScript;

    public List<string> baseColors = new List<string> { "Blue", "Green", "Red", "Yellow", "Pink", "Purple" };

    public GameObject blockPrefab;

    public List<Sprite> blockSprites;

    private Block[,] grid; // 2D array for the grid
    public Dictionary<string, Sprite> defaultIcons;
    public Dictionary<string, Sprite> smallIcons;
    public Dictionary<string, Sprite> mediumIcons;
    public Dictionary<string, Sprite> largeIcons;

    // Add any necessary sprite variables for each icon here
    public Sprite blueDefaultIcon;
    public Sprite greenDefaultIcon;
    public Sprite redDefaultIcon;
    public Sprite yellowDefaultIcon;
    public Sprite pinkDefaultIcon;
    public Sprite purpleDefaultIcon;

    public Sprite blueSmallIcon;
    public Sprite greenSmallIcon;
    public Sprite redSmallIcon;
    public Sprite yellowSmallIcon;
    public Sprite pinkSmallIcon;
    public Sprite purpleSmallIcon;


    public Sprite blueMediumIcon;
    public Sprite greenMediumIcon;
    public Sprite redMediumIcon;
    public Sprite yellowMediumIcon;
    public Sprite pinkMediumIcon;
    public Sprite purpleMediumIcon;

    // Public variables for large icons
    public Sprite blueLargeIcon;
    public Sprite greenLargeIcon;
    public Sprite redLargeIcon;
    public Sprite yellowLargeIcon;
    public Sprite pinkLargeIcon;
    public Sprite purpleLargeIcon;

    // Similarly, add variables for medium and large icons.
    public static BoardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (shuffleMessage != null)
            shuffleMessage.gameObject.SetActive(false);

        // Cache the CameraManager reference
        cameraScript = CameraManager.Instance;
        if (cameraScript == null)
        {
            Debug.LogError("Camera script not found! Make sure a GameObject with the CameraManager script exists in the scene.");
            return;
        }

        // Cache the cell size value
        cellSize = cameraScript.cellSize;

        // Initialize icon dictionaries
        InitializeIconDictionaries();

        // Generate the game board
        GenerateBoard();
    }
    private void InitializeIconDictionaries()
    {
        defaultIcons = new Dictionary<string, Sprite>
    {
        { "Blue", blueDefaultIcon },
        { "Green", greenDefaultIcon },
        { "Red", redDefaultIcon },
        { "Yellow", yellowDefaultIcon },
        { "Pink", pinkDefaultIcon },
        { "Purple", purpleDefaultIcon }
    };

        smallIcons = new Dictionary<string, Sprite>
    {
        { "Blue", blueSmallIcon },
        { "Green", greenSmallIcon },
        { "Red", redSmallIcon },
        { "Yellow", yellowSmallIcon },
        { "Pink", pinkSmallIcon },
        { "Purple", purpleSmallIcon }
    };

        mediumIcons = new Dictionary<string, Sprite>
    {
        { "Blue", blueMediumIcon },
        { "Green", greenMediumIcon },
        { "Red", redMediumIcon },
        { "Yellow", yellowMediumIcon },
        { "Pink", pinkMediumIcon },
        { "Purple", purpleMediumIcon }
    };

        largeIcons = new Dictionary<string, Sprite>
    {
        { "Blue", blueLargeIcon },
        { "Green", greenLargeIcon },
        { "Red", redLargeIcon },
        { "Yellow", yellowLargeIcon },
        { "Pink", pinkLargeIcon },
        { "Purple", purpleLargeIcon }
    };
    }
    // In BoardManager.cs, add:

    // Then modify CreateBlock like this:
    private void CreateBlock(int row, int column)
    {
        // Instantiate the block.
        GameObject blockObj = Instantiate(blockPrefab, transform);
        blockObj.transform.localPosition = new Vector3(column * cellSize, -row * cellSize, 0);

        // Pick a random color index.
        int randomIndex = Random.Range(0, baseColors.Count);
        string baseColor = baseColors[randomIndex];

        // Instead of picking a sprite from blockSprites and using its name,
        // pick the default icon for that color.
        Sprite defaultSprite = defaultIcons[baseColor];

        // Get the Block component and set properties.
        Block block = blockObj.GetComponent<Block>();
        block.SetBlock(baseColor, defaultSprite, new Vector2Int(row, column));

        grid[row, column] = block;
    }


    private void GenerateBoard()
    {
        // Initialize the grid array
        grid = new Block[rows, columns];

        // Ensure cameraScript is valid
        if (cameraScript == null)
        {
            Debug.LogError("Camera script not found! Make sure a GameObject with the camera script exists in the scene.");
            return;
        }

        // Populate the board with blocks
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                CreateBlock(i, j); // Use helper function
            }
        }
        UpdateAllGroupIcons();

    }



    public void HandleBlockClick(Block clickedBlock)
    {
        List<Block> matchedBlocks = GetConnectedBlocks(clickedBlock);

        if (matchedBlocks.Count >= 2)
        {
            StartCoroutine(RemoveRefillAndShuffle(matchedBlocks));
        }
        else
        {
            Debug.Log("No match found.");
        }
    }
    private IEnumerator RemoveRefillAndShuffle(List<Block> matchedBlocks)
    {
        // Update the icons for visual feedback before removal.
        UpdateGroupIcons(matchedBlocks);

        // [Optional pause for animation/feedback]
        yield return new WaitForSeconds(0.2f);

        // Remove all matched blocks.
        foreach (Block block in matchedBlocks)
        {
            if (block != null)
            {
                grid[block.gridPosition.x, block.gridPosition.y] = null;
                Destroy(block.gameObject);
            }
        }

        yield return null; // ensure blocks are destroyed

        // Refill the board.
        yield return StartCoroutine(RefillBoard());

        // Check for deadlock, etc.
        if (IsBoardInDeadlock())
        {
            Debug.Log("Deadlock detected! Shuffling...");
            yield return StartCoroutine(SmartShuffleBoardWithDelay());
        }
    }



    private List<Block> GetConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        bool[,] visited = new bool[rows, columns];
        string targetColor = startBlock.blockColor;

        FloodFill(startBlock.gridPosition.x, startBlock.gridPosition.y, targetColor, connectedBlocks, visited);

        return connectedBlocks;
    }
    private void FloodFill(int startX, int startY, string targetColor, List<Block> connectedBlocks, bool[,] visited)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            Vector2Int pos = stack.Pop();
            int x = pos.x;
            int y = pos.y;

            if (x < 0 || x >= rows || y < 0 || y >= columns || visited[x, y])
                continue;

            Block currentBlock = grid[x, y];
            if (currentBlock == null || currentBlock.blockColor != targetColor)
                continue;

            visited[x, y] = true;
            connectedBlocks.Add(currentBlock);

            stack.Push(new Vector2Int(x - 1, y));
            stack.Push(new Vector2Int(x + 1, y));
            stack.Push(new Vector2Int(x, y - 1));
            stack.Push(new Vector2Int(x, y + 1));
        }
    }




    private IEnumerator RefillBoard()
    {
        // Start refill routines for all columns concurrently.
        for (int column = 0; column < columns; column++)
        {
            StartCoroutine(RefillColumn(column));
        }
        // Wait long enough for all columns to finish their animations.
        yield return new WaitForSeconds(0.6f);
        UpdateAllGroupIcons();

    }

    private Sprite GetIconForGroupSize(string color, int groupSize)
    {
        // Ensure the base color exists.
        if (!defaultIcons.ContainsKey(color))
            return null;

        // Apply the rules:
        if (groupSize <= 4)
        {
            return defaultIcons[color];
        }
        else if (groupSize <= 7) // 5-7
        {
            return smallIcons[color];  // first icon
        }
        else if (groupSize <= 9) // 8-9
        {
            return mediumIcons[color]; // second icon
        }
        else // groupSize >= 10
        {
            return largeIcons[color];  // third icon
        }
    }






    private IEnumerator RefillColumn(int column)
    {
        int emptyCount = 0; // Count empty spaces

        // Move existing blocks down
        for (int row = rows - 1; row >= 0; row--)
        {
            if (grid[row, column] == null)
            {
                emptyCount++;
            }
            else if (emptyCount > 0)
            {
                Block block = grid[row, column];
                grid[row, column] = null;
                grid[row + emptyCount, column] = block;

                Vector3 targetPosition = new Vector3(column * cellSize, -(row + emptyCount) * cellSize, 0);
                StartCoroutine(MoveBlock(block, targetPosition));

                block.gridPosition = new Vector2Int(row + emptyCount, column); // Update grid position
            }
        }

        // Generate new blocks at the top
        for (int i = 0; i < emptyCount; i++)
        {
            CreateNewBlock(i, column);
        }

        yield return null; // Allow frame update
    }

    private void UpdateGroupIcons(List<Block> group)
    {
        if (group.Count < 2)
            return; // Do nothing if not a valid group

        // Use the base color of the first block (all blocks have the same base color).
        string baseColor = group[0].blockColor;
        Sprite newIcon = GetIconForGroupSize(baseColor, group.Count);

        foreach (Block block in group)
        {
            block.spriteRenderer.sprite = newIcon;
        }
    }
    private void UpdateAllGroupIcons()
    {
        bool[,] visited = new bool[rows, columns];

        // Loop over every cell in the grid.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Skip if already visited or if there's no block.
                if (visited[i, j] || grid[i, j] == null)
                    continue;

                // Get the connected group starting from this block.
                List<Block> group = new List<Block>();
                FloodFill(i, j, grid[i, j].blockColor, group, visited);

                // Update the icons for the group if it meets the criteria.
                // (Groups of 1-4 should remain default, so only update if 5 or more.)
                if (group.Count >= 5)
                {
                    UpdateGroupIcons(group);
                }
            }
        }
    }

    private void CreateNewBlock(int row, int column)
    {
        // Instantiate a new block.
        GameObject blockObj = Instantiate(blockPrefab, transform);
        // Set its spawn position (off the top of the board).
        Vector3 spawnPosition = new Vector3(column * cellSize, (rows + 2) * cellSize, 0);
        blockObj.transform.localPosition = spawnPosition;

        // Get the Block component.
        Block newBlock = blockObj.GetComponent<Block>();

        // Pick a random color index from baseColors.
        int randomIndex = Random.Range(0, baseColors.Count);
        string baseColor = baseColors[randomIndex];

        // Use the default icon for the chosen base color.
        Sprite defaultSprite = defaultIcons[baseColor];

        // Set the block with the base color, default icon, and its grid position.
        newBlock.SetBlock(baseColor, defaultSprite, new Vector2Int(row, column));
        grid[row, column] = newBlock;

        // Animate the block falling into its position.
        Vector3 targetPosition = new Vector3(column * cellSize, -row * cellSize, 0);
        StartCoroutine(MoveBlock(newBlock, targetPosition));
    }


    private IEnumerator MoveBlock(Block block, Vector3 targetPosition)
    {
        if (block == null) yield break;

        float duration = 0.3f; // Movement duration
        float elapsedTime = 0f;
        Vector3 startPosition = block.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // Smoothstep easing function

            block.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        if (block != null)
            block.transform.localPosition = targetPosition;
    }

    private bool IsBoardInDeadlock()
    {
        return !HasAvailableMoves(); // If no moves available, it's a deadlock
    }

    private bool HasAvailableMoves()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (CanMatch(i, j))
                {
                    Debug.Log($"Valid move found at ({i}, {j}) with color {grid[i, j]?.blockColor}");
                    return true;
                }
            }
        }
        Debug.Log("No valid moves found.");
        return false;
    }

    private bool CanMatch(int x, int y)
    {
        string color = grid[x, y]?.blockColor;
        if (string.IsNullOrEmpty(color)) return false;

        // Check adjacent blocks
        if (x + 1 < rows && grid[x + 1, y]?.blockColor == color) return true;
        if (y + 1 < columns && grid[x, y + 1]?.blockColor == color) return true;
        if (x - 1 >= 0 && grid[x - 1, y]?.blockColor == color) return true;
        if (y - 1 >= 0 && grid[x, y - 1]?.blockColor == color) return true;

        return false;
    }

    private IEnumerator SmartShuffleBoardWithDelay()
    {
        if (shuffleMessage != null)
        {
            shuffleMessage.text = "Shuffling...";
            shuffleMessage.gameObject.SetActive(true);
        }

        int attempts = 0;
        bool shuffleSuccessful;

        do
        {
            List<Block> blocks = FlattenGrid();
            ShuffleList(blocks);
            RebuildGrid(blocks);

            shuffleSuccessful = HasAvailableMoves();
            attempts++;

            Debug.Log($"Shuffle attempt {attempts}: {shuffleSuccessful}");
            yield return new WaitForSeconds(0.5f); // Allow animations to complete
        }
        while (!shuffleSuccessful && attempts < 10);

        if (!shuffleSuccessful)
        {
            Debug.LogError("Deadlock persists after 10 attempts. Regenerating board...");
            GenerateBoard(); // Reset board
        }

        if (shuffleMessage != null)
            shuffleMessage.gameObject.SetActive(false);
    }



    private List<Block> FlattenGrid()
    {
        List<Block> blockList = new List<Block>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] != null)
                    blockList.Add(grid[i, j]);
            }
        }
        return blockList;
    }

    private void ShuffleListWithAnimation(List<Block> list)
    {
        Debug.Log("Starting shuffle...");

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            // Swap positions in the list
            (list[i], list[j]) = (list[j], list[i]);
        }

        Debug.Log("Shuffle completed.");
    }



    private IEnumerator AnimateShuffle(Block block, Vector3 targetPosition)
    {
        if (block == null) yield break;

        float duration = 0.5f; // Increased duration for visual clarity
        float elapsedTime = 0f;
        Vector3 startPosition = block.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            block.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        block.transform.localPosition = targetPosition;
        Debug.Log($"Animation completed for block at {targetPosition}");
    }


    private void RebuildGrid(List<Block> shuffledBlocks)
    {
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (index < shuffledBlocks.Count)
                {
                    Block block = shuffledBlocks[index];
                    grid[i, j] = block;
                    block.gridPosition = new Vector2Int(i, j);

                    Vector3 targetPosition = new Vector3(j * cellSize, -i * cellSize, 0);
                    block.transform.localPosition = targetPosition;

                    index++;
                }
            }
        }
        Debug.Log("Grid rebuilt.");
    }


    // Helper function to rebuild grid from shuffled list


    private void ShuffleGrid()
    {
        for (int i = rows - 1; i > 0; i--)
        {
            for (int j = columns - 1; j > 0; j--)
            {
                int randRow = Random.Range(0, i + 1);
                int randCol = Random.Range(0, j + 1);

                (grid[i, j], grid[randRow, randCol]) = (grid[randRow, randCol], grid[i, j]);

                if (grid[i, j] != null)
                    grid[i, j].gridPosition = new Vector2Int(i, j);
                if (grid[randRow, randCol] != null)
                    grid[randRow, randCol].gridPosition = new Vector2Int(randRow, randCol);
            }
        }
    }

    private void ShuffleList(List<Block> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            // Swap blocks
            (list[i], list[j]) = (list[j], list[i]);
        }
        Debug.Log("List shuffled.");
    }




}








