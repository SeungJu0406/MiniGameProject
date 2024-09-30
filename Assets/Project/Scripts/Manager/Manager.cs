using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Manager 
{
    public static CardManager Card { get {  return CardManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }
}
