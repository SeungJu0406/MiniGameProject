using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pool 
{
    public static HitUIPool HitUI { get { return HitUIPool.Instance; } }
    public static BattleFieldPool BattleField {  get { return BattleFieldPool.Instance; } }
}
