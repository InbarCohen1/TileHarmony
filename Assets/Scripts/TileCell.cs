using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int _cellCoordinates {  get;  set; }
    public Tile _tile {  get; set; }

    public bool _isEmpty => _tile == null;

    public bool _isOccupied => _tile != null; // TODO: REMOVE??

}
