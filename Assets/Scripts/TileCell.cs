using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int _cellCoordinates {  get;  set; }
    public Tile _tile {  get; set; }

    public bool IsOccupied => _tile != null; // TODO: REMOVE??
}
