using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileStateHandler _state { get; private set; }
    public TileCell _cell { get; private set; }
    public int _number { get; private set; } // TODO:rename -value
}
