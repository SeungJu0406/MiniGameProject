using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDic : MonoBehaviour
{
    public static ItemDic Instance;

    [SerializeField] ItemData[] items;

    public Dictionary<ItemType, ItemData> itemDic = new Dictionary<ItemType, ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Init();
    }

    public void Init()
    {     
        itemDic.Clear();
        foreach (ItemData item in items)
        {
            itemDic.Add(item.type, item);
        }
    }

    public ItemData GetInfo(ItemType type)
    {
        return itemDic[type];
    }
}