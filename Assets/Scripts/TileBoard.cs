using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private TileState[] _tileStates;

    private TileGrid _grid;
    private List<Tile> _tiles;
    private bool _isWaiting;   // Waiting for updated states before animaiting

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }

    public List<Tile> GetAllTiles() => _tiles;

    public void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(_tileStates[0]);
        tile.Spawn(_grid.GetRandomEmptyCell());
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameManager.Instance.RestoreGameState();
            return;
        }

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

        int index = Mathf.Clamp(IndexOf(mergeTo.State) + 1, 0, _tileStates.Length - 1);
        TileState newState = _tileStates[index];

        mergeTo.SetState(newState);
        GameManager.Instance.IncreaseScore(newState.number);
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
}
