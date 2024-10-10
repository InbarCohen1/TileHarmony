using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayerMask; // Ensure this is set to the layer of your tiles
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private TileState[] _tileStates;
    [SerializeField] private InputController _inputController;

    private TileGrid _grid;
    private List<Tile> _tiles;
    private bool _isWaiting;   // Waiting for updated states before animaiting
    private Vector3 worldPosition;
    private Booster _booster;
    private Tile _LockedTile;
    private int _LockedMovesLeft;


    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
        _booster = gameObject.AddComponent<Booster>();
        _booster.Initialize(this);
    }

    public List<Tile> GetAllTiles() => _tiles;

    public void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(_tileStates[0]);
        tile.Spawn(_grid.GetRandomEmptyCell());
        tile.gameObject.layer = LayerMask.NameToLayer("Tiles");

        // Ensure the tile has a collider component
        BoxCollider2D tileCollider = tile.GetComponent<BoxCollider2D>();
        if (tileCollider == null)
        {
            Debug.Log("Adding BoxCollider2D to tile");
            tileCollider = tile.gameObject.AddComponent<BoxCollider2D>(); // Add a BoxCollider2D if none exists
        }

        // Adjust the collider size (scale it up slightly by 1.05x)
        tileCollider.size = new Vector2(tileCollider.size.x * 1.05f, tileCollider.size.y * 1.05f);
        Debug.Log($"Collider size after scaling: {tileCollider.size}");

        // Check if the tile is within the camera's viewport ////// TODO: Remove this code
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(tile.transform.position);
        bool isInView = viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
        Debug.Log($"Created tile at position: {tile.transform.position}, Viewport position: {viewportPosition}, In view: {isInView}");

        _tiles.Add(tile);
    }

    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }

        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }

    private void Update()
    {
        if (_isWaiting) return;

        HandleToolUsage();
        HandleGameControls();
    }

    private void HandleToolUsage() // TODO: Refactor    
    {
        if (ToolManager.Instance.IsAnyToolActive())
        {
            Debug.Log($"Inside HandleToolUsage ");
            if (ToolManager.Instance.IsToolActive(ToolManager.ToolType.RemoveTile) && Input.GetMouseButtonDown(0))
            {
                bool tileRemoved = RemoveTileAtMousePosition();
                if (tileRemoved)
                {
                    ToolManager.Instance.DeactivateTool();
                }
                return;
            }

            if (ToolManager.Instance.IsToolActive(ToolManager.ToolType.LockTile) && Input.GetMouseButtonDown(0))
            {
                bool tileFrozen = LockTileAtMousePosition();
                if (tileFrozen)
                {
                    ToolManager.Instance.DeactivateTool();
                }
                return;
            }

            if (ToolManager.Instance.IsToolActive(ToolManager.ToolType.Shuffle) && Input.GetMouseButtonDown(0))
            {
                ShuffleTiles();
                ToolManager.Instance.DeactivateTool();
                return;
            }

            if (ToolManager.Instance.IsToolActive(ToolManager.ToolType.Booster) && Input.GetMouseButtonDown(0))
            {
                bool tileBoostern = HandleBoosterUsage();
                if (tileBoostern)
                {
                    ToolManager.Instance.DeactivateTool();
                }
                return;
            }

        }
    }

    private void HandleGameControls()
    {
        var keyDirectionMap = new Dictionary<KeyCode, (Vector2Int direction, int startX, int incrementX, int startY, int incrementY)>
        {
            { KeyCode.W, (Vector2Int.up, 0, 1, 1, 1) },
            { KeyCode.UpArrow, (Vector2Int.up, 0, 1, 1, 1) },
            { KeyCode.A, (Vector2Int.left, 1, 1, 0, 1) },
            { KeyCode.LeftArrow, (Vector2Int.left, 1, 1, 0, 1) },
            { KeyCode.S, (Vector2Int.down, 0, 1, _grid.Height - 2, -1) },
            { KeyCode.DownArrow, (Vector2Int.down, 0, 1, _grid.Height - 2, -1) },
            { KeyCode.D, (Vector2Int.right, _grid.Width - 2, -1, 0, 1) },
            { KeyCode.RightArrow, (Vector2Int.right, _grid.Width - 2, -1, 0, 1) }
        };

        foreach (var keyDirection in keyDirectionMap)
        {
            if (Input.GetKeyDown(keyDirection.Key))
            {
                SaveCurrentGameState();
                Move(keyDirection.Value.direction, keyDirection.Value.startX, keyDirection.Value.incrementX, keyDirection.Value.startY, keyDirection.Value.incrementY);
                if (_LockedTile != null && _LockedMovesLeft > 0)
                {
                    _LockedMovesLeft--;
                    if (_LockedMovesLeft == 0)
                    {
                        _LockedTile.IsLocked = false;
                        _LockedTile = null;
                    }
                }
                break;
            }
        }
    }
    private void SaveCurrentGameState() // TODO: refactore
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        List<int> values = new List<int>();

        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                Tile tile = _grid.GetCell(x, y).Tile;

                if (tile != null)
                {
                    positions.Add(new Vector2Int(x, y));
                    values.Add(tile.State.number);
                }
            }
        }

        GameManager.Instance.SaveGameState(new GameState(positions, values, GameManager.Instance.Score));
    }

    private bool Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool ischangedBoard = false;

        for (int x = startX; x >= 0 && x < _grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.Height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell.IsOccupied)
                {
                    ischangedBoard |= MoveTile(cell.Tile, direction);
                }
            }
        }

        if (ischangedBoard)
        {
            StartCoroutine(WaitForChanges());
        }

        return ischangedBoard;
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        if (tile.IsLocked)
        {
            Debug.Log("Tile is locked and cannot be moved.");
            return false;
        }

        TileCell newCell = null;
        TileCell adjacentCell = _grid.GetAdjacentCell(tile.Cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell.IsOccupied)
            {
                if (CanMerge(tile, adjacentCell.Tile))
                {
                    MergeTiles(tile, adjacentCell.Tile);
                    return true;
                }

                break;
            }

            newCell = adjacentCell;
            adjacentCell = _grid.GetAdjacentCell(adjacentCell, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile mergeFrom, Tile mergeTo)
    {
        return mergeFrom.State == mergeTo.State && !mergeTo.IsLocked;
    }

    private void MergeTiles(Tile mergeFrom, Tile mergeTo)
    {
        _tiles.Remove(mergeFrom);
        mergeFrom.Merge(mergeTo.Cell);

        TileState newState = UpdateTileState(mergeTo);
        GameManager.Instance.IncreaseScore(newState.number);
    }

    public TileState UpdateTileState(Tile tile)
    {
        int index = Mathf.Clamp(IndexOf(tile.State) + 1, 0, _tileStates.Length - 1);
        TileState newState = _tileStates[index];
        tile.SetState(newState);
        return newState;
    }
    private int IndexOf(TileState state)
    {
        return Array.IndexOf(_tileStates, state);
    }

    private IEnumerator WaitForChanges()
    {
        _isWaiting = true;

        yield return new WaitForSeconds(0.1f); // Same duration as of animation

        _isWaiting = false;

        UnlockAllTiles();

        if (!IsGridFull())
        {
            CreateTile();
        }

        if (IsGameOver())
        {
            GameManager.Instance.GameOver();
        }
    }

    private void UnlockAllTiles()
    {
        foreach (var tile in _tiles)
        {
            tile.IsLocked = false;
        }
    }

    private bool IsGridFull()
    {
        return _tiles.Count == _grid.Size;
    }

    public bool IsGameOver()
    {
        if (!IsGridFull())
        {
            return false;
        }

        return !IsAnyMergesAvailable();
    }
    private bool IsAnyMergesAvailable()
    {
        foreach (var tile in _tiles)
        {
            if (IsMergesAvailable(tile))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsMergesAvailable(Tile tile)
    {
        TileCell[] adjacentCells = new TileCell[]
        {
        _grid.GetAdjacentCell(tile.Cell, Vector2Int.up),
        _grid.GetAdjacentCell(tile.Cell, Vector2Int.down),
        _grid.GetAdjacentCell(tile.Cell, Vector2Int.left),
        _grid.GetAdjacentCell(tile.Cell, Vector2Int.right)
        };

        foreach (TileCell adjacentCell in adjacentCells)
        {
            if (adjacentCell != null && CanMerge(tile, adjacentCell.Tile))
            {
                return true;
            }
        }

        return false;
    }

    public void RestoreGameState(GameState gameState)
    {
        ClearBoard();

        for (int i = 0; i < gameState.TilePositions.Count; i++)
        {
            Vector2Int position = gameState.TilePositions[i];
            int value = gameState.TileValues[i];
            Tile tile = Instantiate(_tilePrefab, _grid.transform);
            tile.SetState(_tileStates[IndexOf(value)]);
            tile.Spawn(_grid.GetCell(position));
            _tiles.Add(tile);
        }

        GameManager.Instance.SetScore(gameState.Score);
    }

    private int IndexOf(int value)
    {
        for (int i = 0; i < _tileStates.Length; i++)
        {
            if (_tileStates[i].number == value)
            {
                return i;
            }
        }
        return 0;
    }
    public Tile GetTileAt(int x, int y)
    {
        return _grid.GetCell(x, y).Tile;
    }
    public void ShuffleTiles()
    {
        List<Tile> shuffledTiles = new List<Tile>(_tiles);
        System.Random rng = new System.Random();
        int n = shuffledTiles.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tile value = shuffledTiles[k];
            shuffledTiles[k] = shuffledTiles[n];
            shuffledTiles[n] = value;
        }

        // Clear the board without destroying the tiles
        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }


        // Clear the _tiles list to avoid duplicates
        _tiles.Clear();

        // Re-add the shuffled tiles to the board
        foreach (Tile tile in shuffledTiles)
        {
            TileCell randomCell = _grid.GetRandomEmptyCell();
            tile.Spawn(randomCell);
            _tiles.Add(tile);
        }
    }

    private bool RemoveTileAtMousePosition() 
    {
        Vector3 mousePosition = Input.mousePosition;
        Debug.Log($"Mouse Position (Screen): {mousePosition}");

        // Convert mouse position to world coordinates with a fixed Z value
        worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10.0f)); // Adjust Z as needed
        Debug.Log($"Converted World Position: {worldPosition}");
        worldPosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition, tileLayerMask);

        if (hitCollider != null)
        {
            Debug.Log($"Hit Collider: {hitCollider.name}, Bounds: {hitCollider.bounds}");
            Tile tile = hitCollider.GetComponent<Tile>();
            if (tile != null)
            {
                Debug.Log($"Tile selected for removal: {tile.name}");
                RemoveTile(tile);
                return true;
            }
            else
            {
                Debug.Log("No Tile component found on the hit object.");
            }
        }
        else
        {
            Debug.Log("No collider hit.");
        }
        return false;
    }


    private void RemoveTile(Tile tile)
    {
        GameManager.Instance.IncreaseScore(tile.State.number); // TODO:Refactor
        _tiles.Remove(tile);
        tile.Cell.Tile = null;
        Destroy(tile.gameObject);
        Debug.Log("Tile removed: " + tile.name);
    }

  
    private bool HandleBoosterUsage()
    {
        Debug.Log("Handle Booster Mode.");
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            Tile selectedTile = hit.collider.GetComponent<Tile>();
            if (selectedTile != null)
            {
                UseBoosterOnTile(selectedTile);
                return true;
            }
        }
        return false;
    }

    public void UseBoosterOnTile(Tile selectedTile)
    {
        if (selectedTile != null)
        {
            TileState newState = UpdateTileState(selectedTile);
        }
    }

    private bool LockTileAtMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10.0f));
        worldPosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

        if (hitCollider != null)
        {
            Tile tile = hitCollider.GetComponent<Tile>();
            if (tile != null)
            {
                LockTile(tile);
                return true;
            }
        }
        return false;
    }

    private void LockTile(Tile tile) // TODO: Refactor
    {
        tile.IsLocked = true;
        // Optionally, change the tile's appearance to indicate it is locked

        _LockedTile = tile;
        _LockedMovesLeft = 15;
    }

    private void UndoLastMove()
    {
        GameManager.Instance.RestoreGameState();
    }

}
