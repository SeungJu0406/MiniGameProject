using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements.Experimental;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] public LinkedList<Card> cards = new LinkedList<Card>();
    [SerializeField] public LinkedList<VillagerCard> villagers = new LinkedList<VillagerCard>();
    [SerializeField] public LinkedList<FoodCard> foods = new LinkedList<FoodCard>();
    [SerializeField] public List<VillagerCard> deadVillagers = new List<VillagerCard>();

    [SerializeField] public float moveSpeed = 10;
    [SerializeField] public float createPosDistance = 2;
    [Header("일차 타이머")]
    [SerializeField] int day;
    public int Day { get { return day; } set { day = value; OnChangeDay?.Invoke(); } }
    public event UnityAction OnChangeDay;

    [SerializeField] float maxDayTime;
    public float MaxDayTime { get { return maxDayTime; } set { maxDayTime = value; } }

    [SerializeField] float curDayTime;
    public float CurDayTime { get { return curDayTime; } set { curDayTime = value; OnChangeCurDayTime?.Invoke(); } }
    public event UnityAction OnChangeCurDayTime;
    [Header("최대 카드")]
    [SerializeField] int cardCap;
    public int CardCap { get { return cardCap; } set { cardCap = value; OnChangeCardCap?.Invoke(); } }
    public event UnityAction OnChangeCardCap;
    [SerializeField] int cardCount;
    public int CardCount { get { return cardCount; } set { cardCount = value; OnChangeCardCount?.Invoke(); } }
    public event UnityAction OnChangeCardCount;
    [Header("코인 개수")]
    [SerializeField] int coinCount;
    public int CoinCount { get { return coinCount; } set { coinCount = value; OnChangeCoinCount?.Invoke(); } }
    public event UnityAction OnChangeCoinCount;
    [SerializeField] int foodCount;
    public int FoodCount { get { return foodCount / 2; } set { foodCount = value; OnChangeFoodCount?.Invoke(); } }
    public event UnityAction OnChangeFoodCount;
    [Space(10)]
    [SerializeField] int villagerCount;
    public int VillagerCount
    {
        get { return villagerCount; }
        set
        {
            villagerCount = value;
            if (villagerCount <= 0)
            {
                Defeat();
            }
        }
    }
    public event UnityAction OnDefeat;

    public bool isMeatTime;
    [HideInInspector] public int cardLayer;
    WaitForSeconds milliSecond = new WaitForSeconds(0.1f);
    WaitForSeconds milliSecond5 = new WaitForSeconds(0.5f);
    Collider[] hits;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Day = 1;

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
        CurDayTime = 0;
        while (true)
        {
            CurDayTime += 0.1f;
            if (CurDayTime > MaxDayTime)
            {
                break;
            }
            yield return milliSecond;
        }
        // 식사시간 시작
        StartCoroutine(StartMealTime());
    }

    IEnumerator StartMealTime()
    {
        Manager.UI.HideTopUI();
        isMeatTime = true;
        FoodCard food = null;
        // 주민들에게 반복
        foreach (VillagerCard villager in villagers)
        {
            // 주민이 배고플 때
            while (villager.model.Satiety > 0 && foods.Count > 0)
            {

                if (food == null)
                {
                    food = foods.First();
                    food.gameObject.layer = food.ignoreLayer;
                    food.InitOrderLayerAllChild(10000);
                }
                float timer = 0;
                while (true)
                {
                    // 해당 주민에게 이동
                    food.transform.position = Vector3.Lerp(food.transform.position, villager.transform.position, moveSpeed * Time.deltaTime);
                    timer += Time.deltaTime;
                    if (timer > 0.5f)
                        break;
                    yield return null;
                }
                food.Use(villager);
                // 음식의 내구도가 모두 닳았다면
                if (food.model.Durability <= 0)
                {
                    Destroy(food.gameObject);
                    food = null;
                }
            }
        }
        if (food != null)
        {
            food.gameObject.layer = food.ignoreLayer;
            food.InitOrderLayerAllChild(0);
        }
        // 주민중 포만도를 못채운 주민 캐싱
        // 포만도를 채운 주민은 다시 포만도 채우기
        foreach (VillagerCard villager in villagers)
        {
            if (villager.model.Satiety > 0)
            {
                deadVillagers.Add(villager);
            }
            else
            {
                villager.model.Satiety = 2;
                villager.model.CurHp += 5;
            }
        }
        // 캐싱한 주민 사망 처리
        foreach (VillagerCard dead in deadVillagers)
        {
            dead.Die();
        }
        // 캐싱값 삭제
        deadVillagers.Clear();

        // 주민이 살아있다면 카드 갯수체크
        if (villagerCount > 0)
        {
            StartCoroutine(CheckCardCount());
        }
        isMeatTime = false;

    }
    IEnumerator CheckCardCount()
    {
        if (CardCount > CardCap)
        {
            // 카드를 버리라는 UI 출력        
            while (CardCount > CardCap)
            {
                Manager.UI.ShowCardOver();
                yield return null;
            }
        }
        // UI 지우기
        Manager.UI.HideCardOver();
        // 상단 UI 나타내기
        Manager.UI.ShowTopUI();
        // 날짜 올리고루프
        Day++;
        StartCoroutine(DayRoutine());
    }


    void Defeat()
    {
        Debug.Log("게임 패배");
        StopAllCoroutines();
        OnDefeat?.Invoke();
    }

    public void AddCardList(Card card)
    {
        cards.AddLast(card);
        CardCount++;
    }
    public void RemoveCardList(Card card)
    {
        cards.Remove(card);
        CardCount--;
    }
    public void AddVillagerList(VillagerCard villager)
    {
        villagers.AddLast(villager);
        VillagerCount++;
    }
    public void RemoveVillgerList(VillagerCard villager)
    {
        villagers.Remove(villager);
        VillagerCount--;
    }
    public void AddFoodList(FoodCard food)
    {
        foods.AddLast(food);
    }
    public void RemoveFoodList(FoodCard food)
    {
        foods.Remove(food);
    }
    public void AddStorage(int cardcap)
    {
        CardCap += cardcap;
    }
    public void RemoveStorage(int cardcap)
    {
        CardCap -= cardcap;
    }
    public void AddCoin()
    {
        CoinCount++;
    }
    public void RemoveCoin()
    {
        CoinCount--;
    }
    public void MoveResultCard(Vector3 origin, Card instanceCard)
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
        float timer = 0;
        while (true)
        {
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            if (instanceCard.IsChoice)
                yield break;
            if (timer > 0.3f)
                yield break;
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
                yield break;
            yield return null;
        }
    }
    protected Vector3 SelectRandomPos(Vector3 originPos)
    {
        Vector2 dir = Random.insideUnitCircle * createPosDistance;

        return originPos + (Vector3)dir;
    }
}


//// 주민이 배고프고 음식이 남아있을때
//while (villager.model.Satiety > 0 && foods.Count > 0)
//{
//    // 첫번째 음식 꺼냄
//    FoodCard food = foods.First();
//    // 음식의 내구도가 다 닳거나 
//    while (food.model.Durability > 0)
//    {

//        food.gameObject.layer = food.ignoreLayer;
//        food.InitOrderLayerAllChild(10000);
//        float timer = 0;
//        while (true)
//        {
//            // 해당 주민에게 이동
//            food.transform.position = Vector3.Lerp(food.transform.position, villager.transform.position, moveSpeed * Time.deltaTime);
//            timer += Time.deltaTime;
//            if (timer > 0.5f)
//                break;
//            yield return null;
//        }
//        // 먹임
//        food.Use(villager);
//        yield return milliSecond;
//    }
//    foods.Remove(food);
//}