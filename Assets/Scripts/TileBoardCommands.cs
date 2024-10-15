using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShuffleCommand : ICommand
{
    private List<Tile> _tiles;
    private TileGrid _grid;
    public ShuffleCommand(TileBoard tileBoard)
    {
        _tiles = tileBoard.Tiles;
        _grid = tileBoard.Grid;
    }

    public bool Execute()
    {
        List<Tile> shuffledTiles = new List<Tile>(_tiles);
        System.Random rng = new System.Random();
        int n = shuffledTiles.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tile value = shuffledTiles[k];
            shuffledTiles[k] = shuffledTiles[n];
            shuffledTiles[n] = value;
        }

        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }

        _tiles.Clear();

        foreach (Tile tile in shuffledTiles)
        {
            TileCell randomCell = _grid.GetRandomEmptyCell();
            tile.Spawn(randomCell);
            _tiles.Add(tile);
        }
        return true;
    }
}
public class UndoCommand : ICommand
{
    private TileBoard _tileBoard;
    private GameState _gameState;
    private Tile _tilePrefab;
    private TileGrid _grid;
    private TileState[] _tileStates;

    public UndoCommand(TileBoard tileBoard) //TODO: find alternative
    {
        _tileBoard = tileBoard;
        _gameState = GameManager.Instance.SavedGameState;
        _tilePrefab = tileBoard.TilePrefab;
        _grid = tileBoard.Grid;
        _tileStates = tileBoard.TileStates;
    }

    public bool Execute()
    {
        ClearBoard();

        for (int i = 0; i < _gameState.TilePositions.Count; i++)
        {
            Vector2Int position = _gameState.TilePositions[i];
            int value = _gameState.TileValues[i];
            Tile tile = UnityEngine.Object.Instantiate(_tilePrefab, _grid.transform);
            tile.SetState(_tileStates[IndexOf(value)]);
            tile.Spawn(_grid.GetCell(position));
            _tileBoard.Tiles.Add(tile);
        }

        GameManager.Instance.SetScore(_gameState.Score);
        return true;
    }

    private void ClearBoard()
    {
        foreach (Tile tile in _tileBoard.Tiles)
        {
            UnityEngine.Object.Destroy(tile.gameObject);
        }
        _tileBoard.Tiles.Clear();
    }

    private int IndexOf(int value)
    {
        for (int i = 0; i < _tileStates.Length; i++)
        {
            if (_tileStates[i].number == value)
            {
                return i;
            }
        }
        return -1;
    }
}

public class RemoveTileCommand : TileToolCommand
{
    private List<Tile> _tiles;
    public RemoveTileCommand(TileBoard tileBoard)
    {
        _tiles = tileBoard.Tiles;
    }
    protected override void ExecuteLogic(Tile tile)
    {
        GameManager.Instance.IncreaseScore(tile.State.number); // TODO:Refactor
        _tiles.Remove(tile);
        tile.Cell.Tile = null;
        UnityEngine.Object.Destroy(tile.gameObject);
    }
}

public class LockTileCommand : TileToolCommand
{
    private TileBoard _tileBoard;

    public LockTileCommand(TileBoard tileBoard)
    {
        _tileBoard = tileBoard;
    }

    protected override void ExecuteLogic(Tile tile)
    {
        tile.IsLocked = true;

        _tileBoard.SetLockedTile(tile, 15);
    }
}

public class BoosterCommand : TileToolCommand
{
    private TileState[] _tileStates;

    public BoosterCommand(TileBoard tileBoard) 
    {
        _tileStates = tileBoard.TileStates;
    }

    protected override void ExecuteLogic(Tile tile)
    {
        int index = Mathf.Clamp(IndexOf(tile.State) + 1, 0, _tileStates.Length - 1);
        TileState newState = _tileStates[index];
        tile.SetState(newState);
    }
    private int IndexOf(TileState state)
    {
        return Array.IndexOf(_tileStates, state);
    }
}


