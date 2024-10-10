using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    private GameState _savedGameState;

    private const string HiScore = "HiScore";
    public bool IsGameStarted { get; private set; } = false;
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; } = 0;
    [SerializeField] private CanvasGroup _gameOver;

    [Header("TileBoard")]
    [SerializeField] private TileBoard _gameBoard;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        IsGameStarted = true;
        NewGame();
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

    public void GameOver()
    {
        _gameBoard.enabled = false;
        _gameOver.interactable = true;

        StartCoroutine(Fade(_gameOver, 1f, 1f));
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

    //private void Update()  //TODO:To Remove?
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ToolManager.Instance.ActivateTool(ToolManager.ToolType.RemoveTile);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        ToolManager.Instance.ActivateTool(ToolManager.ToolType.Freeze);
    //    }
    //}

}

