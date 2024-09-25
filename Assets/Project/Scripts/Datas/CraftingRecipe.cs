using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CraftingRecipe")]
public class CraftingRecipe : ScriptableObject 
{ 
    [Header("재료 아이템")]
    [SerializeField] public CraftingItemInfo[] reqItems;

    [Header("결과 아이템")]
    [SerializeField] public CraftingItemInfo resultItem;
}

[System.Serializable]
public struct CraftingItemInfo
{
    [SerializeField] public CardData item;
    [SerializeField] public int count;

    public CraftingItemInfo(CardData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}