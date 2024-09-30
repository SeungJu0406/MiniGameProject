using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] public List<Card> cards = new List<Card>();
    [SerializeField] public List<Card> villagers = new List<Card>();
    [SerializeField] public float moveSpeed = 10;
    [SerializeField] public float createPosDistance = 2;
    [SerializeField] public int day;
    public int Day { get { return day; } set { day = value; OnChangeDay?.Invoke(); } }
    public event UnityAction OnChangeDay;
    [SerializeField] public float maxDayTime;
    public float MaxDayTime { get { return maxDayTime; } set { maxDayTime = value; } }
    [SerializeField] public float curDayTime;
    public float CurDayTime {  get { return curDayTime; } set { curDayTime = value; OnChangeCurDayTime?.Invoke(); } }
    public event UnityAction OnChangeCurDayTime;
    


    [HideInInspector] public int cardLayer;
    WaitForSeconds milliSecond = new WaitForSeconds(0.1f);
    Collider[] hits;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        curDayTime = maxDayTime;

        // 배열 초과시 배열 두배 만드는 로직 추가
        hits = new Collider[50];
        cardLayer = LayerMask.GetMask("Card");
    }
    private void Start()
    {
        StartCoroutine(DayRoutine());
    }

    
    IEnumerator DayRoutine()
    {
        while (true)
        {
            CurDayTime += 0.1f;
            if (CurDayTime > MaxDayTime)
            {
                CurDayTime = 0;
                Day++;
            }
            yield return milliSecond;
        }
    }





    public void AddCardList(Card card)
    {
        cards.Add(card);
    }
    public void AddVillagerList(VillagerCard villager)
    {
        villagers.Add(villager);
    }
    public void MoveResultCard(Vector3 origin,Card instanceCard)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(instanceCard.transform.position, createPosDistance, hits, cardLayer);
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i] == null) break;
            Card other = hits[i].GetComponent<Card>();
            if (other.model.BottomCard.model.data == instanceCard.model.data)
            {
                instanceCard.InitInStack(other.model.BottomCard);
                return;
            }
        }
        Vector3 pos = SelectRandomPos(origin);
        StartCoroutine(MoveCardRoutine(instanceCard, pos));
    }

    IEnumerator MoveCardRoutine(Card instanceCard, Vector3 pos)
    {
        while (true)
        {
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }
    protected Vector3 SelectRandomPos(Vector3 originPos)
    {
        Vector2 dir = Random.insideUnitCircle * createPosDistance;

        return originPos + (Vector3)dir;
    }
}
