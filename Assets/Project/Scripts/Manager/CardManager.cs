using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements.Experimental;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] public LinkedList<Card> cards = new LinkedList<Card>();
    [SerializeField] public LinkedList<VillagerCard> villagers = new LinkedList<VillagerCard>();
    [SerializeField] public LinkedList<FoodCard> foods = new LinkedList<FoodCard>();

    [SerializeField] public float moveSpeed = 10;
    [SerializeField] public float createPosDistance = 2;

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
    [Header("음식 량")]
    [SerializeField] int foodCount;
    public int FoodCount { get { return foodCount; } set { foodCount = value; OnChangeFoodCount?.Invoke(); } }
    public event UnityAction OnChangeFoodCount;
    [Space(10)]
    [SerializeField] int villagerCount;
    public int VillagerCount
    {
        get { return villagerCount; }
        set
        {
            villagerCount = value;
            OnChangeVillagerCount?.Invoke();
            if (villagerCount <= 0)
            {
                Manager.Game.CheckDefeat();
            }
        }
    }
    public event UnityAction OnChangeVillagerCount;

    [HideInInspector] public int cardLayer;
    WaitForSeconds milliSecond = new WaitForSeconds(0.1f);
    WaitForSeconds milliSecond5 = new WaitForSeconds(0.5f);
    Collider[] hits;
    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        

        // 배열 초과시 배열 두배 만드는 로직 추가
        hits = new Collider[50];
        cardLayer = LayerMask.GetMask("Card");
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
        FoodCount += food.model.Durability;
    }
    public void RemoveFoodList(FoodCard food)
    {
        foods.Remove(food);
        FoodCount -= food.model.Durability;
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
