using System;
using System.Collections;
using UnityEngine;

public class Util
{
    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max+1);
    }
}
