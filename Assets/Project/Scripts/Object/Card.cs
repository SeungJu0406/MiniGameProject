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
        rb.drag = 50;
        topCard = this;
        cardLayer = LayerMask.NameToLayer("Card");
        ignoreLayer = LayerMask.NameToLayer("IgnoreCollider");      
    }
    private void Start()
    {
        AddCombineList();
    }

    private void Update()
    {
        if (parentCard != null) 
        {
            TraceParent();
        }
    }
    void TraceParent()
    {
        Vector3 parentPos = parentCard.transform.position;
        Vector3 pos = new Vector3(parentPos.x, parentPos.y - 0.4f, parentPos.z);
        transform.position = Vector3.Lerp(transform.position, pos, DragNDrop.Instance.dragSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (collision.gameObject.layer == cardLayer)
        {
            Card parent = collision.gameObject.GetComponent<Card>();
            if (topCard == parent.topCard) return;
            if (parent.ChildCard != null) return;
            // 부모 자식 카드 지정
            ChangeTopChild(parent.topCard);
            parentCard = parent;
            parent.ChildCard = this; 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (topCard == parent.topCard) return;
            if (parent.ChildCard != null) return;
            // 부모 자식 카드 지정
            ChangeTopChild(parent.topCard);
            parentCard = parent;
            parent.ChildCard = this;
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
        ChangeTopChild(this);
        ClickChild();     
    }
    void ClickChild()
    {
        gameObject.layer = ignoreLayer;
        rb.velocity = Vector3.zero;    
        if (ChildCard != null) 
        {
            ChildCard.ClickChild();
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
    void UnClickChild()
    {
        gameObject.layer = cardLayer;
        if (ChildCard != null)
        {
            ChildCard.UnClickChild();
        }
    }


    public void TryCombine()
    {
        if (ingredients.Count <= 0) return;
        if (ingredients.Count == 1 && ingredients[0].count <=1)
            return;
    
        ingredients.Sort((s1, s2) => s1.item.type.CompareTo(s2.item.type));
        string key = RecipeDic.Instance.GetKey(ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingItemInfo result = Dic.Recipe.GetValue(key);
            Debug.Log($"{result.item.itemName} , {result.count}");
      
            CreateResultCard(result.item.prefab, result.count);
        }
        else
        {
            Debug.Log("조합 없음");
        }
    }

    void ChangeTop(Card top)
    {
        if (top != topCard)
        {
            RemoveCombineList();
            topCard = top;
            AddCombineList();
        }
    }
    void ChangeTopChild(Card top)
    {   
        ChangeTop(top);
        if (ChildCard != null)
        {
            ChildCard.ChangeTopChild(top);
        }
    }

    void AddCombineList()
    {
        topCard.AddIngredient(data);
    }

    void RemoveCombineList()
    {
        topCard.RemoveIngredient(data);
    }
    void AddIngredient(CardData data)
    {
        if (ingredients.Any(ingredients => ingredients.item.Equals(data)))
        {
            int index = ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
            CraftingItemInfo findCard = ingredients[index];
            findCard.count++;
            ingredients[index] = findCard;
        }
        else
        {
            ingredients.Add(new CraftingItemInfo(data, 1));
        }
        TryCombine();
    }
    void RemoveIngredient(CardData data)
    {
        int index = ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
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
        TryCombine();
    }

    void CreateResultCard(Card result, int count)
    {
        if (result == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10), 0);
            Instantiate(result, randomPos , transform.rotation);
        }
    }
}
