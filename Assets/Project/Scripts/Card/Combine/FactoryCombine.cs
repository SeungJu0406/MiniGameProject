using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCombine : CardCombine
{
    int factoryListCount;

    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += AddFactoryList;
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void CompleteCreate()
    {
        
    }


    void AddFactoryList()
    {
        model.ingredients.Clear();
        AddCombineChild(model.Card);
        AddFactoryListChild(model.Card);
    }

    void AddCombineChild(Card reqCard)
    {
        model.TopCard.combine.AddIngredient(reqCard.model.data);
    }
    void AddFactoryListChild(Card reqCard)
    {

    }










    protected void TryFactoryCombine()
    {
        //if (model.ChildCard != null && model.data.isFactory)
        //{
        //    model.ingredients.Clear(); // ����Ʈ �ʱ�ȭ
        //    AddIngredient(model.data); // ����ī�� ����Ʈ �Է�
        //    AddIngredient(model.ChildCard.model.data); // �ڽ�ī�� ����Ʈ �Է�         
        //}
    }
    void StopFactoryCreate()
    {
        //if(model.ChildCard == null)
        //{
        //    StopCreate();
        //}
    }
} 
