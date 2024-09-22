using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private TileRow[] _rows;
    public TileCell[] Cells { get; private set; }

    public int Size => Cells.Length; 
    public int Height => _rows.Length; 
    public int Width => Size / Height; 


    private void Awake()
    {
        _rows = GetComponentsInChildren<TileRow>();
        Cells = GetComponentsInChildren<TileCell>();

        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i].CellCoordinates = new Vector2Int(i % Width, i / Width);
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
            return _rows[y].Cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.CellCoordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }


    public TileCell GetRandomEmptyCell() 
    {
        int index = Random.Range(0, Size);

        while (Cells[index].IsOccupied)
        {
            index = (index + 1) % Size;
        }

        return Cells[index];
    }
}
