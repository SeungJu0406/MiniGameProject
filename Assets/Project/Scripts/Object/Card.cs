using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum CardType { Water, Rock,  Wood, Gravel, Gress }
public class Card :MonoBehaviour
{
    [SerializeField] public CardData data;

    [SerializeField] public Card childObject;

    [SerializeField] List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();



    public void TryCombine(CardType type)
    {
        CardData obj = Dic.Card.GetValue(CardType.Rock);
        AddIngredint(obj);
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
    