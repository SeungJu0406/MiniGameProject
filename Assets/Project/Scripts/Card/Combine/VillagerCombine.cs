using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void PostProcessing()
    {
        if (model.ParentCard != null)
        {
            model.ParentCard.model.BottomCard = model.BottomCard;
        }
    }  
}
