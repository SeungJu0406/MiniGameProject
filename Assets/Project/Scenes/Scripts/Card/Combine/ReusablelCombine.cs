using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReusablelCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected void OnDisable()
    {
        
    }
    public override void PostProcessing()
    {
        if (model.ParentCard != null) 
        {
            model.ParentCard.model.BottomCard = model.BottomCard;
        }
    }
}
