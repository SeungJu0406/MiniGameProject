using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    [Header("���� Ÿ�̸�")]
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
        // �Ļ�ð� ����

        StartSettleUp();
    }

    void StartSettleUp()
    {
        Manager.UI.HideTopUI();
        Manager.UI.ShowLeftDownPopUpUI();

        sb.Clear();
        sb.Append($"{Day}��° �� ��");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"�ֹο��� ���� ����");
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
        // �ֹε鿡�� �ݺ�
        foreach (VillagerCard villager in Manager.Card.villagers)
        {
            // �ֹ��� ����� ��
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
                    // �ش� �ֹο��� �̵�
                    food.transform.position = Vector3.Lerp(food.transform.position, villager.transform.position, Manager.Card.moveSpeed * Time.deltaTime);
                    timer += Time.deltaTime;
                    if (timer > 0.5f)
                        break;
                    yield return null;
                }
                food.Use(villager);
                // ������ �������� ��� ��Ҵٸ�
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
        // �ֹ��� �������� ��ä�� �ֹ� ĳ��
        // �������� ä�� �ֹ��� �ٽ� ������ ä���
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
        // ĳ���� �ֹ� ��� ó��
        foreach (VillagerCard dead in Manager.Card.deadVillagers)
        {
            dead.Die();
        }
        // ĳ�̰� ����
        Manager.Card.deadVillagers.Clear();

        // �ֹ��� ����ִٸ� ī�� ����üũ
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
            // ī�带 ������� UI ���        
            while (Manager.Card.CardCount > Manager.Card.CardCap)
            {
                sb.Clear();
                sb.Append("����Ϸ��� ī�� �Ǹ�");
                Manager.UI.UpdatePopUpUIMainText(sb);
                sb.Clear();
                sb.Append($"{Manager.Card.CardCount - Manager.Card.CardCap}���� ī�尡 �ʰ��Ǿ����ϴ�");
                Manager.UI.UpdatePopUpUIButtonText(sb);
                yield return null;
            }
        }
        // ��¥ �ø������
        Day++;
        StartCoroutine(DayRoutine());
    }
}
