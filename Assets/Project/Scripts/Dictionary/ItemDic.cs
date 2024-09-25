using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDic : MonoBehaviour
{
    public static ItemDic Instance;

    [SerializeField] ItemData[] items;

    public Dictionary<ItemType, ItemData> dic = new Dictionary<ItemType, ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Init();
    }

    public void Init()
    {     
        dic.Clear();
        foreach (ItemData item in items)
        {
            dic.Add(item.type, item);
        }
    }

    public ItemData GetValue(ItemType key)
    {
        return dic[key];
    }
}