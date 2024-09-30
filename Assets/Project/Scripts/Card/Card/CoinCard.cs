using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCard :Card
{
    protected override void Start()
    {
        Manager.Card.AddCoin();
    }

    protected override void OnDisable()
    {
        Manager.Card.RemoveCoin();
    }
}
