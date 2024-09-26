using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombine : MonoBehaviour
{
    [SerializeField] CardModel model;

    private void Awake()
    {
        model = GetComponent<CardModel>();

        model.OnChangeTopBefore += RemoveCombineList;
        model.OnChangeTopAfter += AddCombineList;
    }
    public void AddCombineList()
    {
        model.TopCard.combine.AddIngredient(model.data);
    }

    public void RemoveCombineList()
    {
        model.TopCard.combine.RemoveIngredient(model.data);
    }
    void AddIngredient(CardData data)
    {
        if (model.ingredients.Any(ingredients => ingredients.item.Equals(data)))
        {
            int index = model.ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
            CraftingItemInfo findCard = model.ingredients[index];
            findCard.count++;
            model.ingredients[index] = findCard;
        }
        else
        {
            model.ingredients.Add(new CraftingItemInfo(data, 1));
        }
        TryCombine();
    }
    void RemoveIngredient(CardData data)
    {
        int index = model.ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
        if (model.ingredients[index].count <= 1)
        {
            model.ingredients.RemoveAt(index);
        }
        else
        {
            CraftingItemInfo findCard = model.ingredients[index];
            findCard.count--;
            model.ingredients[index] = findCard;
        }
        TryCombine();
    }
    public bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1)
            return false;

        model.ingredients.Sort((s1, s2) => s1.item.type.CompareTo(s2.item.type));
        string key = RecipeDic.Instance.GetKey(model.ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingItemInfo result = Dic.Recipe.GetValue(key);

            Debug.Log($"{result.item.itemName} , {result.count}");
            CreateResultCard(result.item.prefab, result.count);
            return true;
        }
        else
        {
            return false;
        }
    }
    void CreateResultCard(Card result, int count)
    {
        if (result == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10), 0);
            Instantiate(result, randomPos, transform.rotation);
        }
    }
}
