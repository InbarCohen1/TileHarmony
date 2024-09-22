using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int CellCoordinates {  get;  set; }
    public Tile Tile {  get; set; }
    public bool IsOccupied => Tile != null; 
}
