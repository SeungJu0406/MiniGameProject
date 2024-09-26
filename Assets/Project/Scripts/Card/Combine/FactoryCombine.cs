using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeChild += TryFactoryCombine;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void CompleteCreate()
    {
        
    }

    protected void TryFactoryCombine()
    {
        if (model.ChildCard != null && model.data.isFactory)
        {
            model.ingredients.Clear(); // 리스트 초기화
            AddIngredient(model.data); // 본인카드 리스트 입력
            AddIngredient(model.ChildCard.model.data); // 자식카드 리스트 입력         
        }
    }
} 
