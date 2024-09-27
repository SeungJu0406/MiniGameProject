using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CardSpriteController))]
public class CardModel : MonoBehaviour
{
    [SerializeField] public CardData data;
    [SerializeField] int sortOrder;
    public int SortOrder {  get { return sortOrder; } set { sortOrder = value; OnChangeSortOrder?.Invoke(); } }
    public event UnityAction OnChangeSortOrder;

    [SerializeField] Card card;
    public Card Card { get { return card; } set { card = value; } }
    [Space(10)]
    [Header("Stack")]
    [SerializeField] Card topCard;
    public Card TopCard
    {
        get { return topCard; }
        set
        {
            if (topCard != value)
            {
                OnChangeTopBefore?.Invoke();
                topCard = value;
                OnChangeTopAfter?.Invoke();
            }
        }
    }

    public event UnityAction OnChangeTopBefore;
    public event UnityAction OnChangeTopAfter;


    [SerializeField] Card parentCard;
    public Card ParentCard { get { return parentCard; } set { parentCard = value; OnChangeParent?.Invoke(); } }
    public event UnityAction OnChangeParent;
    [SerializeField] Card childCard;
    public Card ChildCard { get { return childCard; } set { childCard = value; OnChangeChild?.Invoke(); } }
    public event UnityAction OnChangeChild;
    [SerializeField] Card bottomCard;
    public Card BottomCard { get { return bottomCard; } set { bottomCard = value; OnChangeBottom?.Invoke();  } }
    public event UnityAction OnChangeBottom;

    [SerializeField] public List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();

    bool canCombine = true;
    public bool CanCombine { get { return canCombine; } set { canCombine = value; } }

    [SerializeField ]bool isFactory;
    public bool IsFactory
    {
        get { return isFactory; }
        set
        {
            if (data.isFactory == true)
            {
                isFactory = value;
                OnChangeIsFactory?.Invoke();
            }
        }
    }
    public event UnityAction OnChangeIsFactory;

    [SerializeField] List<Card> factoryList;
    public List<Card> FactoryList { get { return factoryList; } set { factoryList = value; } }

    [Space(10)]
    [Header("�ֹ� �� ����")]
    [SerializeField] int maxHp;
    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    [SerializeField] int curHp;
    public int CurHp { get {return curHp; } set { curHp = value; } }
    [SerializeField] int damage;
    public int Damage { get { return damage; } set { damage = value; } }
    [SerializeField] int satiety = 2 ;
    public int Satiety {  get { return satiety; } set { satiety = value; } }

}
