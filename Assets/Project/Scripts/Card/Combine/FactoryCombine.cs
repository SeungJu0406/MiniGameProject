using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += AddFactoryList;
        model.OnChangeCanFactoryCombine += CombineControll;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected void AddFactoryList()
    {
        model.ingredients.Clear();
        AddFactoryCombineChild(model.Card);
    }

    protected void CombineControll()
    {
        if (!model.CanFactoryCombine)
        {
            if (createRoutine != null)
            {
                StopCoroutine(createRoutine);
                createRoutine = null;
                timerBar.gameObject.SetActive(false);
            }
        }
    }
    public override void CompleteCreate()
    {
        StartCoroutine(CompleteRoutine());
    }
    WaitForSeconds restartDelay = new WaitForSeconds(0.1f);
    IEnumerator CompleteRoutine()
    {
        yield return restartDelay;
        model.IsFactory = false;
        model.CanFactoryCombine = false;
        AddFactoryList();
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
