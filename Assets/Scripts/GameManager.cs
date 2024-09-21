using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{

    [Header("TileBoard")]
    [SerializeField] private TileBoard _gameBoard;

    [SerializeField] private CanvasGroup _gameOver;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;

    public int _score { get; private set; } = 0;

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

    public void GameOver()
    {
        _gameBoard.enabled = false;
        _gameOver.interactable = true;

        StartCoroutine(Fade(_gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f) // TODO: renaming
        
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
        SetScore(_score + points);
    }

    private void SetScore(int score)
    {
        _score = score;
        _scoreText.text = score.ToString();

        SaveHighscore();
    }

    private void SaveHighscore()
    {
        int hiscore = LoadBestScore();

        if (_score > hiscore)
        {
            PlayerPrefs.SetInt("bestScore", _score);
        }
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("bestScore", 0);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
