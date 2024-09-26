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
                model.ChildCard.model.parentCard = model.parentCard; // 자식의 부모를 본인의 부모로 교체    
            }
            model.parentCard.model.ChildCard = model.ChildCard; //부모의 자식을 본인의 자식으로 교체
            RemoveCombineList();    //본인을 조합리스트에서 제거              
        }
        Destroy(gameObject); //게임 오브젝트 삭제
    }
}
