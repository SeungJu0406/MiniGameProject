using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] public Item prefab;
    [SerializeField] public ItemType type;
    [SerializeField] public string itemName;
}
