using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VillagerCard : Card
{
    public override void Die()
    {
        base.Die();
    }

    public override void Click()
    {
        base.Click();
        
        model.CanGetChild = true;
    }
}
