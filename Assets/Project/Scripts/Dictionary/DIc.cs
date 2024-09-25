using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dic
{
    public static ItemDic Item { get { return ItemDic.Instance; } }
    public static RecipeDic Recipe { get { return RecipeDic.Instance; } }
}
