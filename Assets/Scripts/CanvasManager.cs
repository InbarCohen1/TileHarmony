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
    //[SerializeField] private GameObject _canvasToolsButtonsContainer;
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _gameBoard;
    [SerializeField] private CanvasGroup _menuShop;
    [SerializeField] private CanvasGroup _gameOver;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;

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
    public void ShowGameBoard()
    {
       // SetCanvasGroupVisibility(_mainMenu, false);
        _gameBoard.SetActive(true);
        //SetCanvasGroupVisibility(_gameBoard, true);
        //SetCanvasGroupVisibility(_menuShop, false);
        //SetCanvasGroupVisibility(_gameOver, false);
    }

    public void ShowMenuShop()
    {
        SetCanvasGroupVisibility(_mainMenu, false);
        //SetCanvasGroupVisibility(_gameBoard, false);
        SetCanvasGroupVisibility(_menuShop, true);
        SetCanvasGroupVisibility(_gameOver, false);
    }

    public void ShowGameOverScreen()
    {
        SetCanvasGroupVisibility(_mainMenu, false);
        //SetCanvasGroupVisibility(_gameBoard, false);
        SetCanvasGroupVisibility(_menuShop, false);
        SetCanvasGroupVisibility(_gameOver, true);
    }
    private void SetCanvasGroupVisibility(CanvasGroup canvasGroup, bool isVisible)
    {
        if (isVisible)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.gameObject.SetActive(true); // Ensure the GameObject is active
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false); // Disable the GameObject when not in use
        }
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