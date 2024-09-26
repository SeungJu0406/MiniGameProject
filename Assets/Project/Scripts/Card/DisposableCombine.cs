using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void CompleteCreate()
    {
        if (model.parentCard != null) 
        {
            if (model.ChildCard != null)
            {
                model.ChildCard.model.parentCard = model.parentCard; // �ڽ��� �θ� ������ �θ�� ��ü    
            }
            model.parentCard.model.ChildCard = model.ChildCard; //�θ��� �ڽ��� ������ �ڽ����� ��ü
            RemoveCombineList();    //������ ���ո���Ʈ���� ����              
        }
        Destroy(gameObject); //���� ������Ʈ ����
    }
}
