using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] _rows {  get; private set; }
    public TileCell[] _cells { get; private set; }

    public int _size => _cells.Length;
    public int _height => _rows.Length;
    public int _width => _size / _height;
    

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
}
