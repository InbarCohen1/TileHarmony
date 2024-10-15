using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField] private List<Image> _ToolsIconImages;

    [Header("UI Screens")]
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _gameBoard;
    [SerializeField] private CanvasGroup _menuShop;
    [SerializeField] private GameObject _gameOver;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;
    [SerializeField] private TextMeshProUGUI _coinsInGameOverText;

    private void Awake()
    {
        //ShowMainMenu();
       // _mainMenuGO.SetActive(true);
    }

    public void UpdateHiScore(int hiScore)
    {
        _hiScoreText.text = hiScore.ToString();
    }

    public void UpdateCurrentScore(int currentScore)
    {
        _currentScoreText.text = currentScore.ToString();
    }

    public void ShowMainMenu()
    {
        _mainMenuGO.SetActive(true);
    }

    public void ShowGameOverScreen(int reward)
    {
        _gameOver.SetActive(true);
        _coinsInGameOverText.text = $"Congratulations! You've earned {reward} coins!";
        FadeCanvas(_gameOver.GetComponent<CanvasGroup>(), 1f, 1f);
    }

    public void FadeCanvas(CanvasGroup canvasGroup, float to, float duration = 0.5f)
    {
        StartCoroutine(Fade(canvasGroup, to, duration));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float duration)
    {
        float elapsed = 0f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}