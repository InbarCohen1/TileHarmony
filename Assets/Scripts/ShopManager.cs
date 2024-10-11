using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private int _coins;
    [SerializeField] private TMP_Text _coinUI;
    [SerializeField] private ShopItem[] _shopItems;
    [SerializeField] private GameObject[] _shopPanelsGO;
    [SerializeField] private ShopTemplate[] _shopPanels;
    [SerializeField] private Button[] _myPurchaseBtns;

    void Start()
    {
        for (int i = 0; i < _shopItems.Length; i++)
        {
            _shopPanelsGO[i].SetActive(true);
        }

        UpdateCoinUIText();
        LoadPanels();
        CheckPurchaseable();
    }
    private void UpdateCoinUIText()
    {
        _coinUI.text = "Coins: " + _coins.ToString();
    }

    private void CheckPurchaseable()
    {
        for (int i = 0; i < _shopItems.Length; i++)
        {
            _myPurchaseBtns[i].interactable = _coins >= _shopItems[i].BaseCost;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (_coins >= _shopItems[btnNo].BaseCost)
        {
            _coins -= _shopItems[btnNo].BaseCost;
            UpdateCoinUIText();
            CheckPurchaseable();

            //UnlockItem();
        }
    }

    public void AddCoins(int amount)
    {
        _coins += amount;
        UpdateCoinUIText();
        CheckPurchaseable();
    }

    private void LoadPanels()
    {
        for (int i = 0; i < _shopItems.Length; i++)
        {
            _shopPanels[i].TitleTxt.text = _shopItems[i].Title;
            _shopPanels[i].DescriptionTxt.text = _shopItems[i].Description;
            _shopPanels[i].CostTxt.text = "Coins: " + _shopItems[i].BaseCost.ToString();
            _shopPanels[i].Icon = _shopItems[i].Icon;
        }
    }
}
