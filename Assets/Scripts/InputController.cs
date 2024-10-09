using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField] private Button _boosterButton;
    private bool _isBoosterActive = false;
    public bool IsBoosterActive => _isBoosterActive;

    private void Start()
    {
        _boosterButton.onClick.AddListener(OnBoosterButtonClick);
    }

    public void OnBoosterButtonClick()
    {
        _isBoosterActive = true;
        Debug.Log("Booster Mode enabled");
    }

    public void DeactivateBooster()
    {
        _isBoosterActive = false;
        Debug.Log("Booster Mode Disabled");
    }
}
