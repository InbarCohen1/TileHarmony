using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; } = 0;
    [SerializeField] private CanvasGroup _gameOver;

    [Header("TileBoard")]
    [SerializeField] private TileBoard _gameBoard;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;


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
       NewGame();
    }

    public void NewGame()
    {
        // reset score
        SetScore(0);
        _bestScoreText.text = LoadBestScore().ToString();

        // hide game over screen
        _gameOver.alpha = 0f;
        _gameOver.interactable = false;

        // update _gameBoard state
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

    private void SetScore(int newScore)
    {
        Score = newScore;
        _scoreText.text = newScore.ToString();

        SaveHighscore();
    }

    private void SaveHighscore()
    {
        int bestScore = LoadBestScore();

        if (Score > bestScore)
        {
            PlayerPrefs.SetInt("bestScore", Score);
        }
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("bestScore", 0);
    }

    public void GameOver()
    {
        _gameBoard.enabled = false;
        _gameOver.interactable = true;

        StartCoroutine(Fade(_gameOver, 1f, 1f));
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
