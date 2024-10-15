using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ToolManager;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField] private Button[] _toolsBtns;

    [Header("UI Screens")]
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _gameBoardGO;
    [SerializeField] private CanvasGroup _menuShop;
    [SerializeField] private GameObject _gameOver;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;
    [SerializeField] private TextMeshProUGUI _coinsInGameOverText;

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
        _gameBoardGO.GetComponent<CanvasGroup>().enabled = false;
        _gameOver.SetActive(true);
        _gameOver.GetComponent<CanvasGroup>().interactable = true;
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

    public void UpdateAllToolButtonsInteractability()
    {
        int toolTypeCount = System.Enum.GetValues(typeof(ToolType)).Length - 1; // Exclude ToolType.None
        for (int i = 0; i < _toolsBtns.Length && i < toolTypeCount; i++)
        {
            UpdateToolButtonInteractability((ToolType)i);
        }
    }

    public void UpdateToolButtonInteractability(ToolType toolType)
    {
        if (toolType == ToolType.None) return;

        int toolIndex = (int)toolType - 1;

        if (toolIndex >= 0 && toolIndex < _toolsBtns.Length)
        {
            bool hasQuantity = ShopManager.Instance.ShopItems[toolIndex].Quantity > 0;
            bool isNotOnCooldown = ToolManager.Instance.CanUseTool(toolType);
            _toolsBtns[toolIndex].interactable = hasQuantity && isNotOnCooldown;
        }
        else
        {
            Debug.LogWarning($"Tool index {toolIndex} is out of bounds for _toolsBtns array.");
        }
    }
}
