using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDic : MonoBehaviour
{
    public static CardDic Instance;

    [SerializeField] public List<CardData> cards;

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
        foreach (CardData card in cards)
        {
            dic.Add(card.id, card);
        }
    }

    public CardData GetValue(int key)
    {
        return dic[key];
    }
}