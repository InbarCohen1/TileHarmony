using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    //[SerializeField] private TileBoard _tileBoard;
    private TileBoard _tileBoard;

    public void Initialize(TileBoard tileBoard)
    {
        _tileBoard = tileBoard;
    }

    public void UseBooster(Tile selectedTile)
    {
        if (selectedTile != null)
        {
            Debug.Log($"Booster used on tile at position: {selectedTile.transform.position}");
            TileState newState = _tileBoard.UpdateTileState(selectedTile);
        }
    }
}
