using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDic : MonoBehaviour
{
    public static CardDic Instance;

    [SerializeField] CardData[] objects;

    public Dictionary<int, CardData> dic = new Dictionary<int, CardData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Init();
    }

    public void Init()
    {     
        dic.Clear();
        foreach (CardData obj in objects)
        {
            dic.Add(obj.id, obj);
        }
    }

    public CardData GetValue(int key)
    {
        return dic[key];
    }
}