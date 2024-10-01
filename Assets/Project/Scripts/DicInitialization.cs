using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DicInitialization : MonoBehaviour
{
    [SerializeField] bool isInit;
    [SerializeField] CardDic cardDic;
    [SerializeField] RecipeDic recipeDic;

    [ContextMenu("Init")]
    void Init()
    {
        cardDic.cards.Clear();
        string folderPath = "Assets/Project/Prefabs/Datas/Cards";
        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (string guid in guids) 
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CardData cardData = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
            cardDic.cards.Add(cardData);
        }

        recipeDic.recipes.Clear();
        folderPath = "Assets/Project/Prefabs/Datas/Recipes";
        guids = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            RecipeData recipeData = AssetDatabase.LoadAssetAtPath<RecipeData>(assetPath);
            recipeDic.recipes.Add(recipeData);
        }
    }
}
