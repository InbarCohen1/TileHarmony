using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileBoard : Singleton<TileBoard>
{
    public Tile TilePrefab;
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private TileState[] _tileStates;
    [SerializeField] private InputController _inputController;

    private TileGrid _grid;
    public List<Tile> Tiles;
    private bool _isWaiting;   // Waiting for updated states before animaiting
    private Vector3 worldPosition;
    private Tile _LockedTile;
    private int _LockedMovesLeft;

    public Tile LockedTile => _LockedTile;  //TODO: check alterantive
    public int LockedMovesLeft => _LockedMovesLeft; //TODO: check alterantive

    public const string TilesLayerName = "Tiles";

    public TileState[] TileStates => _tileStates;
    public TileGrid Grid => _grid;

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        Tiles = new List<Tile>(16);
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(TilePrefab, _grid.transform);
        tile.SetState(_tileStates[0]);
        tile.Spawn(_grid.GetRandomEmptyCell());
        tile.gameObject.layer = LayerMask.NameToLayer(TilesLayerName);

        BoxCollider2D tileCollider = tile.GetComponent<BoxCollider2D>();
        tileCollider.size = new Vector2(tileCollider.size.x * 1.05f, tileCollider.size.y * 1.05f);

        Tiles.Add(tile);
    }

    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }

        foreach (var tile in Tiles)
        {
            Destroy(tile.gameObject);
        }

        Tiles.Clear();
    }

    private void Update()
    {
        if (_isWaiting) return;

        HandleToolUsage();
        HandleGameControls();
    }

    private void HandleToolUsage()//TODO:move must logic to toolManager
    {
        if (ToolManager.Instance.IsAnyToolActive())
        {
            ICommand command = ToolManager.Instance.GetCommand(this);
            if (command != null && ToolManager.Instance.CanUseTool(ToolManager.Instance.ActiveTool))
            {
                // Check if the tool is Undo or Shuffle
                if (command is UndoCommand || command is ShuffleCommand)
                {
                    bool hasExecuted = command.Execute();
                    if (hasExecuted)
                    {
                        ToolManager.Instance.DeactivateTool();
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    bool hasExecuted = command.Execute();
                    if (hasExecuted)
                    {
                        ToolManager.Instance.DeactivateTool();
                    }
                }
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
    private void SaveCurrentGameState() // TODO: refactore, move to GameManager
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

        GameManager.Instance.SaveGameState(new GameState(positions, values));
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
            GameManager.Instance.OnMoveMade();
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
        Tiles.Remove(mergeFrom);
        mergeFrom.Merge(mergeTo.Cell);

        TileState newState = UpdateTileState(mergeTo);
        GameManager.Instance.IncreaseScore(newState.number);
    }

    private TileState UpdateTileState(Tile tile)
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

    private void UnlockAllTiles() // TODO: Refactor - list instead of one 
    {
        foreach (var tile in Tiles.Where(t => t != _LockedTile))
        {
            tile.IsLocked = false;
        }
    }

    private bool IsGridFull()
    {
        return Tiles.Count == _grid.Size;
    }

    public bool IsGameOver() //TODO:move to gameManager
    {
        if (!IsGridFull())
        {
            return false;
        }

        return !IsAnyMergesAvailable();
    }
    private bool IsAnyMergesAvailable()
    {
        foreach (var tile in Tiles)
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
    } //TODO: Remove
    public Tile GetTileAt(int x, int y) //TODO: Remove
    {
        return _grid.GetCell(x, y).Tile;
    }
    public void SetLockedTile(Tile tile, int movesLeft)
    {
        _LockedTile = tile;
        _LockedMovesLeft = movesLeft;
    }
}
