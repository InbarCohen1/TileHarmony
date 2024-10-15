using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    private int _coins = 300;
    [SerializeField] private TMP_Text _coinUI;
    [SerializeField] private ShopItem[] _shopItems;
    [SerializeField] private GameObject[] _shopPanelsGO;
    [SerializeField] private ShopTemplate[] _shopPanels;
    [SerializeField] private Button[] _myPurchaseBtns;
    [SerializeField] private Button[] _toolsBtns;
  

    void Start()
    {
        for (int i = 0; i < _shopItems.Length; i++)
        {
            _shopItems[i].Quantity = 0;
            _shopPanelsGO[i].SetActive(true);
        }
        _shopItems[1].Quantity = 1; // For Testing
        UpdateCoinUIText();

        
       // LoadToolQuantities(); 
        LoadPanels();
        CheckPurchaseable();
    }

    public void SetCoins(int coins)
    {
        _coins = coins;
        UpdateCoinUIText();
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
            _shopItems[btnNo].Quantity++;
            _shopPanels[btnNo].QuantityTxt.text = _shopItems[btnNo].Quantity.ToString();
            UnlockItem( btnNo);

            UpdateCoinUIText();
            CheckPurchaseable();
        }
    }

    private void UnlockItem(int btnNo)
    {
        _toolsBtns[btnNo].interactable = true;
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
            _shopPanels[i].QuantityTxt.text = _shopItems[i].Quantity.ToString();
            _shopPanels[i].Icon = _shopItems[i].Icon;
        }
        for (int i = 0; i < _toolsBtns.Length; i++)
        {
            _toolsBtns[i].interactable = false;
        }
    }
    public void SaveToolQuantities()
    {
        ToolData data = new ToolData();
        data.Quantities = new int[_shopItems.Length];
        for (int i = 0; i < _shopItems.Length; i++)
        {
            data.Quantities[i] = _shopItems[i].Quantity;
        }

        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/toolData.json", json);
    }
    public void LoadToolQuantities()
    {
        string path = Application.persistentDataPath + "/toolData.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ToolData data = JsonUtility.FromJson<ToolData>(json);
            for (int i = 0; i < _shopItems.Length; i++)
            {
                _shopItems[i].Quantity = data.Quantities[i];
                _toolsBtns[i].interactable = _shopItems[i].Quantity > 0;
            }
        }
    }
}
