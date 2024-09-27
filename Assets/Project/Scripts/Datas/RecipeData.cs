using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RecipeData")]
public class RecipeData : ScriptableObject 
{ 
    [Header("��� ������")]
    [SerializeField] public CraftingItemInfo[] reqItems;

    [Header("��� ������")]
    [SerializeField] public CraftingItemInfo[] resultItem;

    [Space(30)]
    [Header("���� �ð�")]
    [SerializeField] public float craftingTime; 
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