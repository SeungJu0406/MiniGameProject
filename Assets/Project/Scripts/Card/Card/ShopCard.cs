using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCard : Card
{
    protected override void Start()
    {
        if (!IsInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        IsInitInStack = false;
    }
    protected override void OnDisable() { }
}
