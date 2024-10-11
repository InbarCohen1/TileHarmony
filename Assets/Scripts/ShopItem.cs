using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable objects/New Shop Item", order = 1)]
public class ShopItem : ScriptableObject
{
    public string Title;
    public string Description;
    public int BaseCost;
}
