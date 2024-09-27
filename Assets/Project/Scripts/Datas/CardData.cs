using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card")]
public class CardData : ScriptableObject
{
    [SerializeField] public Card prefab;
    [SerializeField] public int id;
    [SerializeField] public string cardName;
    [Space(10)]
    [SerializeField] public bool isFactory;
    [SerializeField] public bool canGetParent;
    [SerializeField] public bool canGetChild;
    [Space(10)]
    [Header("�ֹ� ������")]
    [SerializeField] public bool isVillager;  
    [ShowIf("isVillager")]  
    [SerializeField] public int maxHp;
    [ShowIf("isVillager")]
    [SerializeField] public int damage;
    [Space(10)]
    [Header("�Ҹ� ������ ����")]
    [SerializeField] public bool isConsumable;
    [ShowIf("isConsumable")]
    [SerializeField] public int foodAmount;
    [ShowIf("isConsumable")]
    [SerializeField] public int additonDamage;
    [ShowIf("isConsumable")]
    [SerializeField] public int additionHp;
    
}
    