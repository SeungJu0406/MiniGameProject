using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerCombine : FactoryCombine
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeParent += ChangeFactoryMode;
        ChangeFactoryMode();
    }
    protected override void Start()
    {
        base.Start();
    }

    void ChangeFactoryMode()
    {

    }

}
