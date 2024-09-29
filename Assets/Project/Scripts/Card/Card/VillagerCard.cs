using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VillagerCard : Card
{
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void Click()
    {
        base.Click();
        
        model.CanGetChild = true;
    }
}
