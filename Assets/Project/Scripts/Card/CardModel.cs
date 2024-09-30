using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CardDataInitialization))]
[RequireComponent(typeof(CardSpriteController))]
public class CardModel : MonoBehaviour
{
    [SerializeField] public CardData data;

    [SerializeField] int durability;
    public int Durability {  get { return durability; } set { durability = value; } }
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

    [SerializeField] bool canFactoryCombine;
    public bool CanFactoryCombine { get { return canFactoryCombine; } set { canFactoryCombine = value; OnChangeCanFactoryCombine?.Invoke(); } }
    public event UnityAction OnChangeCanFactoryCombine;
    [SerializeField] Card factoryBottom;
    public Card FactoryBottom { get { return factoryBottom; } set { factoryBottom = value; } }

    [Space(10)]
    [Header("체력, 데미지 등 정보")]
    [SerializeField] int maxHp;
    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    [SerializeField] int curHp;
    public int CurHp { get { return curHp; } 
        set 
        { 
            curHp = value; 
            if(curHp > maxHp)          
                curHp = maxHp;
            OnChangeCurHp?.Invoke(); 
        } 
    }

    public event UnityAction OnChangeCurHp;
    [SerializeField] int damage;
    public int Damage { get { return damage; } set { damage = value; OnChangeDamage?.Invoke(); } }
    public event UnityAction OnChangeDamage;
    [SerializeField] int satiety = 2 ;
    public int Satiety {  get { return satiety; } set { satiety = value; } }

    [HideInInspector] bool canGetParent;
    public bool CanGetParent { get { return canGetParent; } set { canGetParent = value; } }
    [HideInInspector] bool canGetChild;
    public bool CanGetChild { get { return canGetChild; } set {canGetChild = value; } }
    [SerializeField] bool isFight;
    public bool IsFight { get { return isFight; } set { isFight = value; } }

    [SerializeField] bool isAttack;
    public bool IsAttack { get { return isAttack; } set { isAttack = value; } }
    
    private void Awake()
    {
        Durability = data.durability;
        CanGetParent = data.canGetParent;
        CanGetChild = data.canGetChild;
        if (data.maxHp > 0) MaxHp = data.maxHp;
        if (data.maxHp > 0) CurHp = MaxHp;
        if (data.damage > 0) Damage = data.damage;
    }
}
