using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    [Header("일차 타이머")]
    [SerializeField] int day;
    public int Day { get { return day; } set { day = value; OnChangeDay?.Invoke(); } }
    public event UnityAction OnChangeDay;

    [SerializeField] float maxDayTime;
    public float MaxDayTime { get { return maxDayTime; } set { maxDayTime = value; } }

    [SerializeField] float curDayTime;
    public float CurDayTime { get { return curDayTime; } set { curDayTime = value; OnChangeCurDayTime?.Invoke(); } }
    public event UnityAction OnChangeCurDayTime;

    public bool isMeatTime;
    public enum PopUpButtonState { Null,MeatTime, Defeat, CardCounting}
    public PopUpButtonState curButtonState;

    [HideInInspector] public int cardLayer;
    WaitForSeconds milliSecond = new WaitForSeconds(0.1f);
    WaitForSeconds milliSecond5 = new WaitForSeconds(0.5f);
    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Day = 1;
    }

    private void Start()
    {
        StartCoroutine(DayRoutine());
    }
    IEnumerator DayRoutine()
    {
        Manager.UI.ShowTopUI();
        Manager.UI.HideLeftDownPopUpUI();
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

        StartSettleUp();
    }

    void StartSettleUp()
    {
        Manager.UI.HideTopUI();
        Manager.UI.ShowLeftDownPopUpUI();

        sb.Clear();
        sb.Append($"{Day}번째 달 끝");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"주민에게 음식 제공");
        Manager.UI.UpdatePopUpUIButtonText(sb);
        curButtonState = PopUpButtonState.MeatTime;
    }

    public void StartMeatTime()
    {
        if (curButtonState == PopUpButtonState.MeatTime)
        {
            StartCoroutine(StartMealTimeRoutine());
            curButtonState = PopUpButtonState.Null;
        }
    }
    IEnumerator StartMealTimeRoutine()
    {
        Manager.UI.HideLeftDownPopUpUI();
        isMeatTime = true;
        FoodCard food = null;
        // 주민들에게 반복
        foreach (VillagerCard villager in Manager.Card.villagers)
        {
            // 주민이 배고플 때
            while (villager.model.Satiety > 0 && Manager.Card.foods.Count > 0)
            {

                if (food == null)
                {
                    food = Manager.Card.foods.First();
                    food.gameObject.layer = food.ignoreLayer;
                    food.InitOrderLayerAllChild(10000);
                }
                float timer = 0;
                while (true)
                {
                    // 해당 주민에게 이동
                    food.transform.position = Vector3.Lerp(food.transform.position, villager.transform.position, Manager.Card.moveSpeed * Time.deltaTime);
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
        foreach (VillagerCard villager in Manager.Card.villagers)
        {
            if (villager.model.Satiety > 0)
            {
                Manager.Card.deadVillagers.Add(villager);
            }
            else
            {
                villager.model.Satiety = 2;
                villager.model.CurHp += 5;
            }
        }
        // 캐싱한 주민 사망 처리
        foreach (VillagerCard dead in Manager.Card.deadVillagers)
        {
            dead.Die();
        }
        // 캐싱값 삭제
        Manager.Card.deadVillagers.Clear();

        // 주민이 살아있다면 카드 갯수체크
        if (Manager.Card.VillagerCount > 0)
        {
            StartCoroutine(CheckCardCount());
        }
        isMeatTime = false;

    }
    IEnumerator CheckCardCount()
    {
        Manager.UI.ShowLeftDownPopUpUI();
        if (Manager.Card.CardCount > Manager.Card.CardCap)
        {
            // 카드를 버리라는 UI 출력        
            while (Manager.Card.CardCount > Manager.Card.CardCap)
            {
                sb.Clear();
                sb.Append("계속하려면 카드 판매");
                Manager.UI.UpdatePopUpUIMainText(sb);
                sb.Clear();
                sb.Append($"{Manager.Card.CardCount - Manager.Card.CardCap}장의 카드가 초과되었습니다");
                Manager.UI.UpdatePopUpUIButtonText(sb);
                yield return null;
            }
        }
        // 날짜 올리고루프
        Day++;
        StartCoroutine(DayRoutine());
    }
}
