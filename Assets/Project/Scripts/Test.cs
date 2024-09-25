using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Test : MonoBehaviour
{
    List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();
    private void Start()
    {
        ingredients.Clear();
        CardData rock = Dic.Card.GetValue(CardType.Rock);
        AddIngredint(rock);

        CardData water = Dic.Card.GetValue(CardType.Water);
        AddIngredint(water);
        //AddIngredient(water);

        CardData wood = Dic.Card.GetValue(CardType.Wood);
        AddIngredint(wood);

        ingredients.Sort((s1, s2) => s1.item.type.CompareTo(s2.item.type));
        string key = RecipeDic.Instance.GetKey(ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingItemInfo result = Dic.Recipe.GetValue(key);
            Debug.Log($"{result.item.itemName} , {result.count}");
        }
        else
        {
            Debug.Log("조합 없음");
        }
    }
    void AddIngredint(CardData item)
    {
        if (ingredients.Any(ingredients => ingredients.item.Equals(item)))
        {
            int index = ingredients.FindIndex(ingredients => ingredients.item.Equals(item));
            CraftingItemInfo findItem = ingredients[index];
            findItem.count++;
            ingredients[index] = findItem;
        }
        else
        {
            ingredients.Add(new CraftingItemInfo(item, 1));
        }
    }
}
