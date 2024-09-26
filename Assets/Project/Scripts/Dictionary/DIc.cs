using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dic
{
    public static CardDic Card { get { return CardDic.Instance; } }
    public static RecipeDic Recipe { get { return RecipeDic.Instance; } }
}
