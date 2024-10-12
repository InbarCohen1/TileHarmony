using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>
{
    private const string HiScore = "HiScore";
    private const string CoinsKey = "Coins";

    public bool IsGameStarted { get; private set; } = false;
    private GameState _savedGameState;
    public int Score { get; private set; } = 0;
    [SerializeField] private CanvasGroup _gameOver;
    [SerializeField] private CanvasGroup _mainMenu;

    [Header("TileBoard")]
    [SerializeField] private TileBoard _gameBoard;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;

    [Header("Shop Manager")]
    [SerializeField] private ShopManager _shopManager; 


    private void Start()
    {
        IsGameStarted = true; //TODO: Move?
        _mainMenu.interactable = true;
        StartCoroutine(Fade(_mainMenu, 1f, 1f));
        
        int savedCoins = LoadCoins();
        _shopManager.SetCoins(savedCoins);
    }

    public void NewGame()
    {
        SetScore(0);
        _hiScoreText.text = LoadHiScore().ToString();

        // hide game over screen
        _gameOver.alpha = 0f;
        _gameOver.interactable = false;

        _gameBoard.ClearBoard();
        _gameBoard.CreateTile();
        _gameBoard.CreateTile();
        _gameBoard.enabled = true;

        ToolManager.Instance.ResetCooldowns();
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(Score + points);
    }

    public void SetScore(int newScore)
    {
        Score = newScore;
        _scoreText.text = newScore.ToString();

        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int bestScore = LoadHiScore();

        if (Score > bestScore)
        {
            PlayerPrefs.SetInt(HiScore, Score);
        }
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
        _gameBoard.enabled = false;
        _gameOver.interactable = true;

        RewardPlayer();

        StartCoroutine(Fade(_gameOver, 1f, 1f));
        ToolManager.Instance.ResetCooldowns();
    }

    private void RewardPlayer()
    {
        int coins = 0;

        if (Score >= 10000)
        {
            coins = 150;
        }
        else if (Score >= 5000)
        {
            coins = 75;
        }
        else if (Score >= 1000)
        {
            coins = 25;
        }

        if (Score > LoadHiScore())
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

    public void QuitGame()
    {
        IsGameStarted = false;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
}

