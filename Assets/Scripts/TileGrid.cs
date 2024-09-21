using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] _rows {  get; private set; }
    public TileCell[] _cells { get; private set; }

    public int _size => _cells.Length; //TODO: public?
    public int _height => _rows.Length; //TODO: public?
    public int _width => _size / _height; //TODO: public?


    private void Awake()
    {
        _rows = GetComponentsInChildren<TileRow>();
        _cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < _rows.Length; y++)
        {
            var row = _rows[y];
            for (int x = 0; x < row._cells.Length; x++)
            {
                row._cells[x]._cellCoordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
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


    public TileCell GetRandomEmptyCell() // TODO: refactore : index = (index + 1) % size;
    {
        int index = Random.Range(0, _cells.Length); // TODO:  _cells.Length --> _size 
        int startingIndex = index;

        while (_cells[index]._isOccupied)
        {
            index++;

            if (index >= _cells.Length)
            {
                index = 0;
            }

            if(index == startingIndex)
            {
                return null;
            }
        }

        return _cells[index];
    }
}
