using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDic : MonoBehaviour
{
    public static CardDic Instance;

    [SerializeField] CardData[] objects;

    public Dictionary<CardType, CardData> dic = new Dictionary<CardType, CardData>();

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
            dic.Add(obj.type, obj);
        }
    }

    public CardData GetValue(CardType key)
    {
        return dic[key];
    }
}