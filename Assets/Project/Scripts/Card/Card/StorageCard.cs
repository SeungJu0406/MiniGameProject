using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageCard : Card
{
    protected override void Start()
    {
        base.Start();
        Manager.Card.AddStorage(model.data.cardCap);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Manager.Card.RemoveStorage(model.data.cardCap);
    }
}
    