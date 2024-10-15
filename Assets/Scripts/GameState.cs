using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<Vector2Int> TilePositions { get; }
    public List<int> TileValues { get; }
    public int Score { get; }

    public GameState(List<Vector2Int> positions, List<int> values)
    {
        TilePositions = new List<Vector2Int>(positions);
        TileValues = new List<int>(values);
        Score = GameManager.Instance.CurrentScore;
    }
}
