using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCard : Card
{
    protected override void Start()
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        isInitInStack = false;
    }
}
