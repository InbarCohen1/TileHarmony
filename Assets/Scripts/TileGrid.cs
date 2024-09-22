using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private TileRow[] _rows;
    public TileCell[] _cells { get; private set; }

    public int Size => _cells.Length; 
    public int Height => _rows.Length; 
    public int Width => Size / Height; 


    private void Awake()
    {
        _rows = GetComponentsInChildren<TileRow>();
        _cells = GetComponentsInChildren<TileCell>();

        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i]._cellCoordinates = new Vector2Int(i % Width, i / Width);
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return _rows[y]._cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell._cellCoordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }


    public TileCell GetRandomEmptyCell() 
    {
        int index = Random.Range(0, Size);

        while (_cells[index]._isOccupied)
        {
            index = (index + 1) % Size;
        }

        return _cells[index];
    }
}
