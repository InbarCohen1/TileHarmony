using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile _tilePrefab; // TODO:  [Header("Bullets")] [SerializeField] private GameObject _bulletPrefab;
    public GameManager gameManager;

    public TileState[] tileStates;

    private TileGrid _grid;
    private List<Tile> _tiles;
    private bool _isWaiting;   // waiting for updated states before animaiting

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }


    public void ClearBoard()
    {
        foreach (var cell in _grid._cells)
        {
            cell._tile = null;
        }

        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }


    public void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
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
            Move(Vector2Int.down, 0, 1, _grid._height - 2, -1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right, _grid._width - 2, -1, 0, 1);
        }

    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY) //TODO: Renaming ischangedBoard
    {
        bool ischangedBoard = false;

        for (int x = startX; x >= 0 && x < _grid._width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid._height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell._isOccupied)
                {
                    ischangedBoard |= MoveTile(cell._tile, direction);
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
        TileCell adjacentCell = _grid.GetAdjacentCell(tile._cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell._isOccupied)
            {
                if (CanMerge(tile, adjacentCell._tile))
                {
                    MergeTiles(tile, adjacentCell._tile);
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

    private bool CanMerge(Tile a, Tile b)
    {
        return a._state == b._state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b._cell); // a is beening merged to b

        int index = Mathf.Clamp(IndexOf(b._state) + 1, 0, tileStates.Length - 1);
        //TileState newState = tileStates[index];
        int number = b._number * 2; //TODO: REMOVE
        b.SetState(tileStates[index], number);
        //b.SetState(newState);
        //GameManager.Instance.IncreaseScore(newState.number);
        gameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state) //  TODO : refactore
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        _isWaiting = true;

        yield return new WaitForSeconds(0.1f); // same duration as animation

        _isWaiting = false;

        foreach (var tile in _tiles)
        {
            tile.locked = false;
        }

        if (_tiles.Count != _grid._size)
        {
            CreateTile();
        }

        if (IsGameOver())
        {
            gameManager.GameOver();
           // GameManager.Instance.GameOver();
        }
    }

    public bool IsGameOver() //TODO: refactore
    {
        if (_tiles.Count != _grid._size)
        {
            return false;
        }
        //TODO: helpper func - check if any mergeas are available
        foreach (var tile in _tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile._cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile._cell, Vector2Int.down);
            TileCell left = _grid.GetAdjacentCell(tile._cell, Vector2Int.left);
            TileCell right = _grid.GetAdjacentCell(tile._cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up._tile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down._tile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left._tile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right._tile))
            {
                return false;
            }
        }

        return true;
    }
}
