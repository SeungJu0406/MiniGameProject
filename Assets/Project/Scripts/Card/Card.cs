using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



[RequireComponent(typeof(CardModel))]
public class Card : MonoBehaviour
{
    [Header("GetComponent")]
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public CardModel model;
    [SerializeField] public CardCombine combine;

    [Space(30)]
    float stackInterval = 0.4f;
    int cardLayer;
    int ignoreLayer;

    bool isChoice;
    bool isInitInStack;
    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        model = GetComponent<CardModel>();       
        combine = GetComponent<CardCombine>();

        model.Card = this;
        model.OnChangeChild += InitChangeChild;

        rb.drag = 5;
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
    }
    protected virtual void Update()
    {
        if (model.ParentCard != null) 
        {
            TraceParent();
        }
    }
    IEnumerator InitIgnoreColliderRoutine() 
    {       
        gameObject.layer = ignoreLayer;
        yield return new WaitForSeconds(1f);
        gameObject.layer = cardLayer;
    }
    void TraceParent()
    {
        Vector3 parentPos = model.ParentCard.transform.position;
        Vector3 pos = new Vector3(parentPos.x, parentPos.y - stackInterval, parentPos.z);
        transform.position = Vector3.Lerp(transform.position, pos, DragNDrop.Instance.dragSpeed * Time.deltaTime);
    }
    public void InitInStack(Card parent)
    {
        if (!model.data.canGetParent) return;
        if (!parent.model.data.canGetChild) return;
        isInitInStack = true;
        model.TopCard = this;
        model.BottomCard = this;
        model.ParentCard = parent;
        parent.model.ChildCard = this;
        ChangeSortLayerAllChild();
        ChangeTopAllChild(parent.model.TopCard); // 본인 + 자식에게 top 설정
        ChangeBottomAllParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
        if (parent.rb != null) parent.rb.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!model.data.canGetParent) return;
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (model.ParentCard != null) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (!parent.model.data.canGetChild) return;
            if (model.TopCard == parent.model.TopCard) return;
            if (parent.model.ChildCard != null) return;
            // 부모 자식 카드 지정
            model.ParentCard = parent;
            parent.model.ChildCard = this;
            ChangeSortLayerAllChild();
            ChangeTopAllChild(parent.model.TopCard); // 본인 + 자식에게 top 설정           
            ChangeBottomAllParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
            parent.rb.velocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!model.data.canGetParent) return;
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if(model.ParentCard != null) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (!parent.model.data.canGetChild) return;
            if (model.TopCard == parent.model.TopCard) return;
            if (parent.model.ChildCard != null) return;
            // 부모 자식 카드 지정
            model.ParentCard = parent;
            parent.model.ChildCard = this;
            ChangeSortLayerAllChild();
            ChangeTopAllChild(parent.model.TopCard); // 본인 + 자식에게 top 설정           
            ChangeBottomAllParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
        }
    }

    public void InitChangeChild()
    {
        if (boxCollider != null)
        {
            if (model.ChildCard != null)
            {
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.1f);
                boxCollider.isTrigger = true;
            }
            else
            {
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 1f);
                boxCollider.isTrigger = false;
            }
        }
    }
    public void Click()
    {
        if (model.ParentCard != null)
        {
            model.ParentCard.model.ChildCard = null;
            model.ParentCard.ChangeBottomAllParent(model.ParentCard); // 부모 카드들의 바텀을 맞부모카드로 설정           
            model.ParentCard = null;           
        }   
        isChoice = true;
        InitSortLayerAllChild(10000);
        ChangeTopAllChild(this);
        
        ClickAllChild();     
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
    public void UnClick()
    {
        InitSortLayerAllChild(0);
        UnClickAllChild();
        StartCoroutine(UnClickDelayRoutine());
    }
    WaitForSeconds delay = new WaitForSeconds(0.1f);
    IEnumerator UnClickDelayRoutine()
    {
        yield return delay;
        isChoice = false;       
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
    public void ChangeSortLayerAllChild()
    {
        model.SortOrder = model.ParentCard.model.SortOrder + 1;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeSortLayerAllChild();
        }
    }
    public void InitSortLayerAllChild(int order)
    {
        model.SortOrder = order;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeSortLayerAllChild();
        }
    }

}
