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
            model.ingredients.Clear(); // ����Ʈ �ʱ�ȭ
            AddIngredient(model.data); // ����ī�� ����Ʈ �Է�
            AddIngredient(model.ChildCard.model.data); // �ڽ�ī�� ����Ʈ �Է�         
        }
    }
} 
