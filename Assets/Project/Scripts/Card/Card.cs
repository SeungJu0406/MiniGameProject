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
    int cardLayer;
    int ignoreLayer;

    public bool isChoice;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        model = GetComponent<CardModel>();       
        combine = GetComponent<CardCombine>();

        model.card = this;
        model.OnChangeChild += InitChangeChild;

        rb.drag = 50;
        cardLayer = LayerMask.NameToLayer("Card");
        ignoreLayer = LayerMask.NameToLayer("IgnoreCollider");

        StartCoroutine(InitIgnoreColliderRoutine());
    }

    private void Start()
    {          
        model.TopCard = this;
        model.BottomCard = this;
    }
    private void Update()
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
        Vector3 pos = new Vector3(parentPos.x, parentPos.y - 0.4f, parentPos.z);
        transform.position = Vector3.Lerp(transform.position, pos, DragNDrop.Instance.dragSpeed * Time.deltaTime);
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
            ChangeTopChild(parent.model.TopCard); // 본인 + 자식에게 top 설정           
            ChangeBottomParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
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
            ChangeTopChild(parent.model.TopCard); // 본인 + 자식에게 top 설정           
            ChangeBottomParent(model.BottomCard); // 본인 + 부모에게 bottom 설정
        }
    }

    public void InitChangeChild()
    {
        if (model.ChildCard != null)
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
        if (model.ParentCard != null)
        {
            model.ParentCard.ChangeBottomParent(model.ParentCard); // 부모 카드들의 바텀을 맞부모카드로 설정
            model.ParentCard.model.ChildCard = null;
            model.ParentCard = null;           
        }
        isChoice = true;
        ChangeTopChild(this);
        
        ClickChild();     
    }
    void ClickChild()
    {
        gameObject.layer = ignoreLayer;
        rb.velocity = Vector3.zero;    
        if (model.ChildCard != null) 
        {
            model.ChildCard.ClickChild();
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
        if (model.ChildCard != null)
        {
            model.ChildCard.UnClickChild();
        }
    }
    public void ChangeTopChild(Card top)
    {
        model.TopCard = top;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeTopChild(top);
        }
    }
    public void ChangeBottomParent(Card bottom)
    {
        model.BottomCard = bottom;
        if (model.ParentCard != null)
        {
            model.ParentCard.ChangeBottomParent(bottom);
        }
    }

}
