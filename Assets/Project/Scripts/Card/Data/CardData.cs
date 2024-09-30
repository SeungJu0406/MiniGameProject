using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card")]
public class CardData : ScriptableObject
{
    [SerializeField] public Card prefab;
    [SerializeField] public Sprite cardIcon;
    [SerializeField] public int id;
    [SerializeField] public string cardName;
    [SerializeField] public string engName;
    [SerializeField] public int price;
    [SerializeField] public int durability;
    [Space(10)]
    [SerializeField] public bool isFactory;
    [SerializeField] public bool canGetParent;
    [SerializeField] public bool canGetChild;
    [SerializeField] public bool cantMove;
    [Space(10)]
    [Header("주민, 몬스터 데이터")]
    [SerializeField] public bool isVillager;
    [SerializeField] public bool isMonster; 
    [SerializeField] public int maxHp;
    [SerializeField] public int damage;
    [Space(10)]
    [Header("소모 아이템 정보")]
    [SerializeField] public bool isConsumable;
    [ShowIf("isConsumable")]
    [SerializeField] public bool isCoin;
    [ShowIf("isConsumable")]
    [SerializeField] public int foodAmount;
    [ShowIf("isConsumable")]
    [SerializeField] public int additonDamage;
    [ShowIf("isConsumable")]
    [SerializeField] public int additionHp;
    [Space(10)]
    [Header("드랍 카드")]
    [SerializeField] public List<CraftingItemInfo> rewardCards;
    [Space(10)]
    [Header("상점 카드")]
    [SerializeField] public List<CardData> randomCards;
    [Space(10)]
    [Header("창고 카드")]
    [SerializeField] public int cardCap;

    [Header("스택 무시 특수 카드")]
    [SerializeField] public bool isIgnoreStack;


}
    