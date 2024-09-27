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
    [SerializeField] public int durability;
    [Space(10)]
    [SerializeField] public bool isFactory;
    [SerializeField] public bool canGetParent;
    [SerializeField] public bool canGetChild;
    [Space(10)]
    [Header("주민 데이터")]
    [SerializeField] public bool isVillager;  
    [ShowIf("isVillager")]  
    [SerializeField] public int maxHp;
    [ShowIf("isVillager")]
    [SerializeField] public int damage;
    [Space(10)]
    [Header("소모 아이템 정보")]
    [SerializeField] public bool isConsumable;
    [ShowIf("isConsumable")]
    [SerializeField] public int foodAmount;
    [ShowIf("isConsumable")]
    [SerializeField] public int additonDamage;
    [ShowIf("isConsumable")]
    [SerializeField] public int additionHp;
}
    