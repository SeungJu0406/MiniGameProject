using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public enum CardType { Water, Rock, Wood, Gravel, Gress }
public class Card : MonoBehaviour
{
    [Header("GetComponent")]
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] public Rigidbody rb;

    [SerializeField] public CardData data;

    [SerializeField] public Card topCard;
    public Card TopCard { get { return topCard; } set { OnChangeTopBefore?.Invoke() ; topCard = value; OnChangeTopAfter?.Invoke(); } }
    public event UnityAction OnChangeTopBefore;
    public event UnityAction OnChangeTopAfter;
    [SerializeField] public Card parentCard;
    [SerializeField] public Card childCard;
    public Card ChildCard { get { return childCard; } set { childCard = value; OnChangeChild?.Invoke(); } }
    public event UnityAction OnChangeChild;

    [SerializeField] List<CraftingItemInfo> ingredients = new List<CraftingItemInfo>();

    int cardLayer;
    int ignoreLayer;

    public bool isChoice;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        OnChangeChild += InitChangeChild;
        OnChangeTopBefore += RemoveCombineList;
        OnChangeTopAfter += AddCombineList;
        rb.drag = 50;
        topCard = this;
        cardLayer = LayerMask.NameToLayer("Card");
        ignoreLayer = LayerMask.NameToLayer("IgnoreCollider");

        AddIngredient(data);
    }

    private void Update()
    {
        if (parentCard != null) 
        {
            TraceParent();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (collision.gameObject.layer == cardLayer)
        {
            Card parent = collision.gameObject.GetComponent<Card>();
            if (TopCard == parent.TopCard) return;
            if (parent.ChildCard != null) return;
            // 부모 자식 카드 지정
            TopCard = parent.TopCard;
            parentCard = parent;
            parent.ChildCard = this;
            // 조합 배열에 넣기  
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (TopCard == parent.TopCard) return;
            if (parent.ChildCard != null) return;
            // 부모 자식 카드 지정
            TopCard = parent.TopCard;
            parentCard = parent;
            parent.ChildCard = this;

            // 조합 배열에 넣기
        }
    }


    public void InitChangeChild()
    {
        if (ChildCard != null)
        {
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.01f);
            boxCollider.isTrigger = true;
        }
        else
        {
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 1);
            boxCollider.isTrigger = false;
        }
    }
    public void Click()
    {
        if (parentCard != null)
        {
            parentCard.ChildCard = null;
            parentCard = null;           
        }
        isChoice = true;
        ClickChild(this);
        
    }
    public void ClickChild(Card top)
    {
        TopCard = top;
        gameObject.layer = ignoreLayer;
        rb.velocity = Vector3.zero;    
        if (ChildCard != null) 
        {
            ChildCard.ClickChild(top);
        }
    }
    public void UnClick()
    {
        UnClickChild();
        StartCoroutine(UnClickDelayRoutine());
    }
    WaitForSeconds delay = new WaitForSeconds(0.1f);
    IEnumerator UnClickDelayRoutine()
    {
        yield return delay;
        isChoice = false;       
    }
    public void UnClickChild()
    {
        gameObject.layer = cardLayer;
        if (ChildCard != null)
        {
            ChildCard.UnClickChild();
        }
    }
    void TraceParent()
    {
        Vector3 parentPos = parentCard.transform.position;
        Vector3 pos = new Vector3(parentPos.x, parentPos.y - 0.4f, parentPos.z);
        transform.position = Vector3.Lerp(transform.position, pos, DragNDrop.Instance.dragSpeed * Time.deltaTime);
    }

    public void TryCombine(CardType type)
    {
        CardData obj = Dic.Card.GetValue(CardType.Rock);
        AddIngredient(obj);

        ingredients.Sort((s1, s2) => s1.item.type.CompareTo(s2.item.type));
        string key = RecipeDic.Instance.GetKey(ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingItemInfo result = Dic.Recipe.GetValue(key);
            Debug.Log($"{result.item.itemName} , {result.count}");
        }
        else
        {
            Debug.Log("조합 없음");
        }
    }

    void AddCombineList()
    {
        TopCard.AddIngredient(data);
    }

    void RemoveCombineList()
    {
        TopCard.RemoveIngredient(data);
    }
    void AddIngredient(CardData card)
    {
        if (ingredients.Any(ingredients => ingredients.item.Equals(card)))
        {
            int index = ingredients.FindIndex(ingredients => ingredients.item.Equals(card));
            CraftingItemInfo findCard = ingredients[index];
            findCard.count++;
            ingredients[index] = findCard;
        }
        else
        {
            ingredients.Add(new CraftingItemInfo(card, 1));
        }
    }
    void RemoveIngredient(CardData card)
    {
        int index = ingredients.FindIndex(ingredients => ingredients.item.Equals(card));
        if (ingredients[index].count <= 1)
        {
            ingredients.RemoveAt(index);
        }
        else
        {
            CraftingItemInfo findCard = ingredients[index];
            findCard.count--;
            ingredients[index] = findCard;
        }
    }
}
