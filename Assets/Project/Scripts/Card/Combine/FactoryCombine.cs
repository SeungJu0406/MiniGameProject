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
        //    model.ingredients.Clear(); // 리스트 초기화
        //    AddIngredient(model.data); // 본인카드 리스트 입력
        //    AddIngredient(model.ChildCard.model.data); // 자식카드 리스트 입력         
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
