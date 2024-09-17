using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile _tilePrefab; // TODO:  [Header("Bullets")] [SerializeField] private GameObject _bulletPrefab;
    
    public TileState[] tileStates;

    private TileGrid _grid;
    private List<Tile> _tiles;

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }

    private void Start()
    {
        CreateTile();
        CreateTile();
    }


    private void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
    }
}
