using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>
{
    private const string HiScore = "HiScore";
    private const string CoinsKey = "Coins";

    [Header("GameLoop")]
    private int _currentScore;
    private int _hiScore;
    public int CurrentScore => _currentScore;
    public bool IsGameStarted { get; private set; } = false; 
    private GameState _savedGameState;
    public GameState SavedGameState => _savedGameState; 

    [Header("Board")]
    private int _newTilesOnNewGame = 2;


    private void Start()
    {
        IsGameStarted = true;
        CanvasManager.Instance.ShowMainMenu();

        int savedCoins = LoadCoins();
        ShopManager.Instance.SetCoins(savedCoins);
        ShopManager.Instance.LoadToolQuantities();
    }

    public void NewGame()
    {
        _currentScore = 0;
        _hiScore = LoadHiScore();
        CanvasManager.Instance.UpdateHiScore(_hiScore);

        TileBoard.Instance.ClearBoard();
        for (var i = 0; i < _newTilesOnNewGame; i++)
        {
            TileBoard.Instance.CreateTile();
        }

        ToolManager.Instance.ResetCooldowns();
    }

    public void IncreaseScore(int points)
    {
        SetScore(_currentScore + points);
    }

    public void SetScore(int newScore)
    {
        _currentScore = newScore;

        if (_currentScore > _hiScore)
        {
            _hiScore = _currentScore;
            PlayerPrefs.SetInt(HiScore, _hiScore);
            PlayerPrefs.Save();

            CanvasManager.Instance.UpdateHiScore(_hiScore);
        }

        CanvasManager.Instance.UpdateCurrentScore(_currentScore);
    }

    private void SaveHiscore()
    {
        _hiScore = _currentScore;
        PlayerPrefs.SetInt(HiScore, _hiScore);
        PlayerPrefs.Save();
    }

    private int LoadHiScore()
    {
        return PlayerPrefs.GetInt(HiScore, 0);
    }

    public int LoadCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public void OnMoveMade()
    {
        if (IsGameStarted)
        {
            ToolManager.Instance.DecreaseCooldowns();
        }
    }

    public void GameOver()
    {
        int reward = RewardPlayer();
        CanvasManager.Instance.ShowGameOverScreen(reward);
    
        ToolManager.Instance.ResetCooldowns();
    }

    private int RewardPlayer()
    {
        int reward = 0;

        if (_currentScore >= 10000)
        {
            reward = 150;
        }
        else if (_currentScore >= 5000)
        {
            reward = 75;
        }
        else if (_currentScore >= 1000)
        {
            reward = 25;
        }

        if (_currentScore > _hiScore)
        {
            reward += 100; // Breaking record
        }

        ShopManager.Instance.AddCoins(reward);
        SaveCoins(reward);
        return reward;
    }

    private void SaveCoins(int coins)
    {
        int currentCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        PlayerPrefs.SetInt(CoinsKey, currentCoins + coins);
        PlayerPrefs.Save();
    }

    public void SaveGameState(GameState gameState)
    {
        _savedGameState = gameState;
    }
  
    private void OnApplicationQuit()
    {
        ShopManager.Instance.SaveToolQuantities(); 
    }
    public void QuitGame()
    {
        IsGameStarted = false;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

