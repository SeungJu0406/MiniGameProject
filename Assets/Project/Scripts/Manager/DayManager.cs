using Cinemachine;
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
        // �Ļ�ð� ����

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
        sb.Append($"{Day}��° �� ��");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"�ֹο��� ���� ����");
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
        sb.Append("�Ļ� ��. . .");
        Manager.UI.UpdatePopUpUIMainText(sb);
        isMeatTime = true;
        FoodCard food = null;
        CinemachineVirtualCamera focusCam = null;

        // ������ ������ ��ŭ ����
        int deadCount = Manager.Card.VillagerCount - Manager.Card.FoodCount / 2;
        for (int i = 0; i < deadCount; i++) 
        {
            Manager.Card.villagers.Last().Die();
        }
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
                    focusCam = Manager.Camera.GetCamera(CameraType.Focus);
                    focusCam.Follow = food.transform;
                    focusCam.LookAt = food.transform;
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
        // �ֹ��� ����ִٸ� ī�� ����üũ
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
        sb.Append($"�ķ��� �����մϴ�");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append($"{Manager.Card.VillagerCount - Manager.Card.FoodCount/2}���� ���� �׽��ϴ�");
        Manager.UI.UpdatePopUpUIButtonText(sb);      
        curPopUpState = PopUpState.MeatTime;
    }
    void Defeat()
    {
        curPopUpState = PopUpState.Defeat;       
    }
}
