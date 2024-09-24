using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RecipeDic : MonoBehaviour
{
    public static RecipeDic Instance;

    [SerializeField] CraftingRecipe[] recipes;

    public Dictionary<string, CraftingItemInfo> recipeDic = new Dictionary<string, CraftingItemInfo>();

    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Init();
    }    

    private void Init()
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            Array.Sort(recipe.reqItems, (s1, s2) => s1.item.type.CompareTo(s2.item.type));
            recipeDic.Add(GetKey(recipe.reqItems), recipe.resultItem);
        }
    }
    public CraftingItemInfo GetInfo(string key)
    {
        return recipeDic[key];
    }

    public string GetKey(CraftingItemInfo[] reqItems) 
    {
        sb.Clear();
        foreach (CraftingItemInfo item in reqItems)
        {
            sb.Append($"{item.item.itemName}{item.count}");
        }
        return sb.ToString();
    }
}
