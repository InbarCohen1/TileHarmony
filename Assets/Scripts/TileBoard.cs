using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private TileState[] _tileStates;

    //public GameManager gameManager;
    private TileGrid _grid;
    private List<Tile> _tiles;
    private bool _isWaiting;   // waiting for updated states before animaiting

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }

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

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
        }

    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
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
        //int number = mergeTo._number * 2; //TODO: REMOVE
        // mergeTo.SetState(tileStates[index], number);

        mergeTo.SetState(newState);
        GameManager.Instance.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        return Array.IndexOf(_tileStates, state);

        //for (int i = 0; i < tileStates.Length; i++)
        //{
        //    if (state == tileStates[i])
        //    {
        //        return i;
        //    }
        //}

        //return -1;
    }

    private IEnumerator WaitForChanges()
    {
        _isWaiting = true;

        yield return new WaitForSeconds(0.1f); // same duration as animation

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

    public bool IsGameOver() //TODO: refactore
    {
        if (!IsGridFull())
        {
            return false;
        }

        return !IsAnyMergesAvailable();

        ////TODO: helpper func - check if any mergeas are available
        //foreach (var tile in _tiles)
        //{
        //    TileCell up = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
        //    TileCell down = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);
        //    TileCell left = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
        //    TileCell right = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);

        //    if (up != null && CanMerge(tile, up.Tile))
        //    {
        //        return false;
        //    }

        //    if (down != null && CanMerge(tile, down.Tile))
        //    {
        //        return false;
        //    }

        //    if (left != null && CanMerge(tile, left.Tile))
        //    {
        //        return false;
        //    }

        //    if (right != null && CanMerge(tile, right.Tile))
        //    {
        //        return false;
        //    }
        //}

        //return true;
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
}
