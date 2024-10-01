using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


 public enum CardKey { Coin = 2 } 
[RequireComponent(typeof(CardModel))]
public class Card : MonoBehaviour
{
    [Header("참조가 필요함!")]
    [SerializeField] public TextMeshProUGUI hpUI;
    [Space(10)]
    [Header("GetComponent")]
    [SerializeField] public BoxCollider boxCollider;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public CardModel model;
    [SerializeField] public CardCombine combine;
    [Space(10)]

    float stackInterval = 0.4f;
    [HideInInspector] public int cardLayer;
    [HideInInspector] public int ignoreLayer;

    [HideInInspector] bool isChoice;
    public bool IsChoice { get { return isChoice; } set { isChoice = value; OnChangeIsChoice?.Invoke(); } }
    public event UnityAction OnChangeIsChoice;

    public event UnityAction<Card> OnClick;
    public event UnityAction<Card> OnDie;
    protected bool isInitInStack;
    protected StringBuilder sb = new StringBuilder();
    protected virtual void Awake()
    {
        BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
        boxCollider = colliders[1];
        rb = GetComponent<Rigidbody>();
        model = GetComponent<CardModel>();
        combine = GetComponent<CardCombine>();

        model.Card = this;
        model.OnChangeChild += InitCollider;
        model.OnChangeCurHp += UpdateCurHp;

        rb.drag = 50;
        cardLayer = LayerMask.NameToLayer("Card");
        ignoreLayer = LayerMask.NameToLayer("IgnoreCollider");


        StartCoroutine(InitIgnoreColliderRoutine());
    }

    protected virtual void Start()
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        isInitInStack = false;
        Manager.Sound.PlaySFX(Manager.Sound.sfx.combine);
        Manager.Card.AddCardList(this);
    }

    protected virtual void OnDisable()
    {
        Manager.Card.RemoveCardList(this);
    }
    protected virtual void Update()
    {
        if (Manager.Day.isMeatTime)
        {
            if (waitMealTIme == null)
            {
                waitMealTIme = StartCoroutine(WaitMealTime());
            }
        }
        else
        {
            if (model.ParentCard != null)
            {
                TraceParent();
            }
        }
    }
    Coroutine waitMealTIme;
    IEnumerator WaitMealTime()
    {
        while (Manager.Day.isMeatTime)
        {
            IgnoreCollider();
            yield return null;
        }
        InitCollider();
        waitMealTIme = null;
    }

    public IEnumerator InitIgnoreColliderRoutine()
    {
        gameObject.layer = ignoreLayer;
        boxCollider.gameObject.layer = ignoreLayer;
        yield return new WaitForSeconds(1f);
        gameObject.layer = cardLayer;
        boxCollider.gameObject.layer = default;
    }
    void TraceParent()
    {
        Vector3 parentPos = model.ParentCard.transform.position;
        Vector3 pos = new Vector3(parentPos.x, parentPos.y - stackInterval, parentPos.z);
        transform.position = Vector3.Lerp(transform.position, pos, DragNDrop.Instance.dragSpeed * Time.deltaTime);
    }
    public void InitInStack(Card parent)
    {
        if (!model.CanGetParent) return;
        if (!parent.model.CanGetChild) return;
        isInitInStack = true;
        model.TopCard = model.TopCard == null ? this : model.TopCard;      
        model.BottomCard = model.BottomCard == null? this : model.BottomCard;
        model.ParentCard = parent;
        parent.model.ChildCard = this;
        ChangeOrderLayerAllChild();
        ChangeTopAllChild(parent.model.TopCard); // 본인 + 자식에게 top 설정
        ChangeBottomAllParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
        if (parent.rb != null) parent.rb.velocity = Vector3.zero;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {     
        if (DragNDrop.Instance.isClick) return;
        if (!IsChoice) return;
        if (model.ParentCard != null) return;
        if (model.IsFight) return;
        if(model.IsAccessIgnoreStack) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();

            if (parent.model.data.isIgnoreStack)
            {
                parent.IgnoreStack(this);
                return;
            }
            if (model.TopCard == parent.model.TopCard) return;
            if (!model.CanGetParent) return;
            if (!parent.model.CanGetChild) return;
            if (parent.model.ChildCard != null) return;
            
            // 부모 자식 카드 지정
            model.ParentCard = parent;
            parent.model.ChildCard = this;
            ChangeOrderLayerAllChild();
            ChangeTopAllChild(parent.model.TopCard); // 본인 + 자식에게 top 설정           
            ChangeBottomAllParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
            parent.rb.velocity = Vector3.zero;
        }
    }
    public virtual void IgnoreStack(Card card) { }
    public void InitCollider()
    {
        if (model.data.cantMove) return;
        if (boxCollider != null)
        {
            if (model.ChildCard != null)
            {
                IgnoreCollider();
            }
            else
            {
                NotIgnoreCollider();
            }
        }
    }
    public void IgnoreCollider()
    {
        if (model.Card != null)
        {
            boxCollider.gameObject.SetActive(false);
        }

    }
    public void NotIgnoreCollider()
    {
        if (model.Card != null)
        {
            boxCollider.gameObject.SetActive(true);
        }
    }
    public virtual void Click()
    {
        if (model.ParentCard != null)
        {
            model.ParentCard.model.ChildCard = null;
            model.ParentCard.ChangeBottomAllParent(model.ParentCard); // 부모 카드들의 바텀을 맞부모카드로 설정           
            model.ParentCard = null;
        }
        IsChoice = true;
        OnClick?.Invoke(this);
        InitOrderLayerAllChild(10000);
        ChangeTopAllChild(this);
        ClickAllChild();
        model.BottomCard.IgnoreCollider();
        Manager.Sound.PlaySFX(model.data.clip);
    }
    void ClickAllChild()
    {
        gameObject.layer = ignoreLayer;
        rb.velocity = Vector3.zero;
        if (model.ChildCard != null)
        {
            model.ChildCard.ClickAllChild();
        }
    }
    public virtual void UnClick()
    {
        InitOrderLayerAllChild(0);
        UnClickAllChild();
        StartCoroutine(UnClickDelayRoutine());
        model.BottomCard.NotIgnoreCollider();
        Manager.Sound.PlaySFX(Manager.Sound.sfx.unclick);
    }
    WaitForSeconds delay = new WaitForSeconds(0.1f);
    IEnumerator UnClickDelayRoutine()
    {
        yield return delay;
        IsChoice = false;
    }
    void UnClickAllChild()
    {
        gameObject.layer = cardLayer;
        if (model.ChildCard != null)
        {
            model.ChildCard.UnClickAllChild();
        }
    }
    public void ChangeTopAllChild(Card top)
    {
        model.TopCard = top;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeTopAllChild(top);
        }
    }
    public void ChangeBottomAllParent(Card bottom)
    {
        model.BottomCard = bottom;
        if (model.ParentCard != null)
        {
            model.ParentCard.ChangeBottomAllParent(bottom);
        }
    }
    public void ChangeOrderLayerAllChild()
    {
        model.SortOrder = model.ParentCard.model.SortOrder + 1;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeOrderLayerAllChild(); 
        }
    }
    public void InitOrderLayerAllChild(int order)
    {
        model.SortOrder = order;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeOrderLayerAllChild();
        }
    }

    public virtual void Die()
    {
        OnDie?.Invoke(this);
        DropRewardCard();
        combine.Delete();
    }

    void DropRewardCard()
    {
        CraftingItemInfo rewardCardInfo = model.data.rewardCards[Util.Random(0, model.data.rewardCards.Count - 1)];
        for (int i = 0; i < rewardCardInfo.count; i++)
        {
            Card rewardCard = Instantiate(rewardCardInfo.item.prefab, transform.position, transform.rotation);
            bool canStack = Manager.Card.InsertStackResultCard(rewardCard);
            if (!canStack)
            {
                Manager.Card.RandomSpawnCard(transform.position, rewardCard);
            }
        }
    }
    void UpdateCurHp()
    {
        sb.Clear();
        sb.Append(model.CurHp);
        hpUI.SetText(sb);
    }
}
