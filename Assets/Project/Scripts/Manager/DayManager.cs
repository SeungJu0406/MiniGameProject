using Cinemachine;
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
    public float MaxDayTime { get { return maxDayTime; } set { maxDayTime = value; OnChangeMaxDayTime?.Invoke(); } }
    public event UnityAction OnChangeMaxDayTime;

    [SerializeField] float curDayTime;
    public float CurDayTime { get { return curDayTime; } set { curDayTime = value; OnChangeCurDayTime?.Invoke(); } }
    public event UnityAction OnChangeCurDayTime;

    public bool isMeatTime;
    public enum PopUpState { Null,MeatTime, Fail ,Defeat, CardCounting}
    public PopUpState curPopUpState;

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
        Manager.UI.HidePopUpUI();
        CurDayTime = 0;
        curPopUpState = PopUpState.Null;
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
        Manager.Sound.PlaySFX(Manager.Sound.sfx.settle);
        Manager.Input.CanClick = false;
        Manager.UI.HideTopUI();
        Manager.UI.HideLeftUI();
        Manager.UI.ShowPopUpUI();
        sb.Clear();
        sb.Append($"{Day}번째 달 끝");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"주민에게 음식 제공");
        Manager.UI.UpdatePopUpUIButtonText(sb);
        if (Manager.Card.FoodCount/2 < Manager.Card.VillagerCount)
        {
            curPopUpState = PopUpState.Fail;          
        }
        else
        {
            curPopUpState = PopUpState.MeatTime;
        }
    }

    IEnumerator StartMealTimeRoutine()
    {
        sb.Clear();
        Manager.UI.UpdatePopUpUIButtonText(sb);
        sb.Append("식사 중. . .");
        Manager.UI.UpdatePopUpUIMainText(sb);
        isMeatTime = true;
        FoodCard food = null;
        CinemachineVirtualCamera focusCam = null;

        // 음식이 부족한 만큼 죽음
        int deadCount = Manager.Card.VillagerCount - Manager.Card.FoodCount / 2;
        for (int i = 0; i < deadCount; i++) 
        {
            Manager.Card.villagers.Last().Die();
        }
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
                    focusCam = Manager.Camera.GetCamera(CameraType.Focus);
                    focusCam.Follow = food.transform;
                    focusCam.LookAt = food.transform;
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
        focusCam = Manager.Camera.GetCamera(CameraType.Main);

        if (food != null)
        {
            food.gameObject.layer = food.cardLayer;
            food.InitOrderLayerAllChild(0);           
        }
        foreach(VillagerCard villager in Manager.Card.villagers)
        {       
            villager.model.Satiety = 2;
            villager.model.CurHp += 5;
        }
        // 주민이 살아있다면 카드 갯수체크
        if (Manager.Card.VillagerCount > 0)
        {
            StartCoroutine(CheckCardCount());
        }
        else
        {
            Defeat();
        }
        isMeatTime = false;

    }
    IEnumerator CheckCardCount()
    {
        Manager.Input.CanClick = true;
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

    public void StartMeatTime()
    {
        if (curPopUpState == PopUpState.MeatTime)
        {
            StartCoroutine(StartMealTimeRoutine());
            curPopUpState = PopUpState.Null;
        }
        else if (curPopUpState == PopUpState.Fail)
        {
            FailSettleUp();
        }
    }
    void FailSettleUp()
    {
        sb.Clear();
        sb.Append($"식량이 부족합니다");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"{Manager.Card.VillagerCount - Manager.Card.FoodCount/2}명이 굶어 죽습니다");
        Manager.UI.UpdatePopUpUIButtonText(sb);      
        curPopUpState = PopUpState.MeatTime;
    }
    void Defeat()
    {
        curPopUpState = PopUpState.Defeat;       
    }
}
