using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile _tilePrefab; // TODO:  [Header("Bullets")] [SerializeField] private GameObject _bulletPrefab;
    
    public TileState[] tileStates;

    private TileGrid _grid;
    private List<Tile> _tiles;

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }

    private void Start()
    {
        CreateTile();
        CreateTile();
    }


    private void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
    }

    private void Update()
    {
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

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < _grid._width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid._height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell._isOccupied)
                {
                    changed |= MoveTile(cell._tile, direction);
                }
            }
        }

        //if (changed)
        //{
        //    StartCoroutine(WaitForChanges());
        //}
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacentCell = _grid.GetAdjacentCell(tile._cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell._isOccupied)
            {
                //if (CanMerge(tile, adjacentCell._tile))
                //{
                //    MergeTiles(tile, adjacentCell._tile);
                //    return true;
                //}

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
}
