using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]

public class TileState : ScriptableObject
{
    public int number;  //TODO: rename - value
    public Color backgroundColor;
    public Color textColor;
}
