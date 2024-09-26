using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public enum CardType { Water, Rock, Wood, Gravel, Gress }
[RequireComponent(typeof(CardModel))]
[RequireComponent(typeof(CardCombine))]
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

        model.OnChangeChild += InitChangeChild;

        rb.drag = 50;
        cardLayer = LayerMask.NameToLayer("Card");
        ignoreLayer = LayerMask.NameToLayer("IgnoreCollider");

        StartCoroutine(InitIgnoreColliderRoutine());
    }

    private void Start()
    {          
        model.TopCard = this;
    }
    private void Update()
    {
        if (model.parentCard != null) 
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
        Vector3 parentPos = model.parentCard.transform.position;
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
            if (model.TopCard == parent.model.TopCard) return;
            if (parent.model.ChildCard != null) return;
            // 부모 자식 카드 지정
            ChangeTopChild(parent.model.TopCard);
            model.parentCard = parent;
            parent.model.ChildCard = this; 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (DragNDrop.Instance.isClick) return;
        if (!isChoice) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (model.TopCard == parent.model.TopCard) return;
            if (parent.model.ChildCard != null) return;
            // 부모 자식 카드 지정
            ChangeTopChild(parent.model.TopCard);
            model.parentCard = parent;
            parent.model.ChildCard = this;
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
        if (model.parentCard != null)
        {
            model.parentCard.model.ChildCard = null;
            model.parentCard = null;           
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
    void ChangeTopChild(Card top)
    {
        model.TopCard = top;
        if (model.ChildCard != null)
        {
            model.ChildCard.ChangeTopChild(top);
        }
    }
}
