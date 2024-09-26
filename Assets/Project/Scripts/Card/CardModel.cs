using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardModel : MonoBehaviour
{
    [SerializeField] public CardData data;
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

    [SerializeField] public Card parentCard;
    [SerializeField] Card childCard;
    public Card ChildCard { get { return childCard; } set { childCard = value; OnChangeChild?.Invoke(); } }
    public event UnityAction OnChangeChild;

    [SerializeField] public List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();


    private void Awake()
    {
        
    }
}
