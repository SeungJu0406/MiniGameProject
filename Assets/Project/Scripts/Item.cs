using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType { Water, Rock,  Wood, Gravel, Gress }
public class Item :MonoBehaviour
{
    [SerializeField] public ItemData data;  
}
    