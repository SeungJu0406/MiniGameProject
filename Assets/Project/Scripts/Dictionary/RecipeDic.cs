using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RecipeDic : MonoBehaviour
{
    public static RecipeDic Instance;

    [SerializeField] CraftingRecipe[] recipes;

    public Dictionary<string, CraftingRecipe> dic = new Dictionary<string, CraftingRecipe>();

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
            Array.Sort(recipe.reqItems, (s1, s2) => s1.item.id.CompareTo(s2.item.id));
            dic.Add(GetKey(recipe.reqItems), recipe);
        }
    }
    public CraftingRecipe GetValue(string key)
    {
        return dic[key];
    }

    public string GetKey(CraftingItemInfo[] reqItems) 
    {
        sb.Clear();
        foreach (CraftingItemInfo item in reqItems)
        {
            sb.Append($"{item.item.cardName}{item.count}");
        }
        return sb.ToString();
    }
}
