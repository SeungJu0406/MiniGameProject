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
    [Header("�ֹ�, ���� ������")]
    [SerializeField] public bool isVillager;
    [SerializeField] public bool isMonster; 
    [SerializeField] public int maxHp;
    [SerializeField] public int damage;
    [Space(10)]
    [Header("�Ҹ� ������ ����")]
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
    [Header("��� ī��")]
    [SerializeField] public List<CraftingItemInfo> rewardCards;
    [Space(10)]
    [Header("���� ī��")]
    [SerializeField] public List<CardData> randomCards;
}
    