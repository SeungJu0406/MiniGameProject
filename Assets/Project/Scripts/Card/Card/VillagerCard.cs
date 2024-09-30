using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VillagerCard : Card
{
    protected override void Start()
    {
        base.Start();
        Manager.Card.AddVillagerList(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Manager.Card.RemoveVillgerList(this);
    }

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
