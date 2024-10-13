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
    private int _coins;
    public int CurrentScore => _currentScore; // TODO: Remove?
    public bool IsGameStarted { get; private set; } = false;
    private GameState _savedGameState;

    [Header("TileBoard")]
    [SerializeField] private TileBoard _gameBoard;
    private int _newTilesOnNewGame = 2;

    [Header("Shop Manager")]
    [SerializeField] private ShopManager _shopManager; 


    private void Start()
    {
        IsGameStarted = true;
        CanvasManager.Instance.ShowMainMenu();

        int savedCoins = LoadCoins();
        _shopManager.SetCoins(savedCoins);
    }

    public void NewGame()
    {
        _currentScore = 0;
        _hiScore = LoadHiScore();
        CanvasManager.Instance.UpdateHiScore(_hiScore);

        //CanvasManager.Instance.ShowGameBoard();
        _gameBoard.ClearBoard();
        for (var i = 0; i < _newTilesOnNewGame; i++)
        {
            _gameBoard.CreateTile();
        }
        _gameBoard.enabled = true; //TODO: Remove?

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
        // _gameBoard.enabled = false; //TODO: Remove?
        CanvasManager.Instance.ShowGameOverScreen();
    
        RewardPlayer();
        ToolManager.Instance.ResetCooldowns();
    }

    private void RewardPlayer()
    {
        int coins = 0;

        if (_currentScore >= 10000)
        {
            coins = 150;
        }
        else if (_currentScore >= 5000)
        {
            coins = 75;
        }
        else if (_currentScore >= 1000)
        {
            coins = 25;
        }

        if (_currentScore > _hiScore)
        {
            coins += 100; // Breaking record
        }

        _shopManager.AddCoins(coins);
        SaveCoins(coins);
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

    public void RestoreGameState()
    {
        if (_savedGameState != null)
        {
            _gameBoard.RestoreGameState(_savedGameState);
        }
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

