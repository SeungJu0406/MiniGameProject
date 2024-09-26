using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardModel : MonoBehaviour
{
    [SerializeField] public CardData data;
    [SerializeField] public Card card;
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
    public Card BottomCard { get { return bottomCard; } set { bottomCard = value; } }

    [SerializeField] public List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();

}
