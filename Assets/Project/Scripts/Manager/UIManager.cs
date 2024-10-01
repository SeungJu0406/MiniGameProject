using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("상단 UI")]
    public Animator topUI;
    [System.Serializable]
    public struct DayTimer
    {
        public Slider timerBar;
        public TextMeshProUGUI dayText;
    }
    [Header("상단 타이머 UI")]
    public DayTimer dayTimer;
    [System.Serializable]
    public struct CardCount
    {
        public Image cardIcon;
        public TextMeshProUGUI cardCountText;
    }
    [Header("카드 갯수 UI")]
    public CardCount cardCount;

    [Header("코인 갯수 UI")]
    public TextMeshProUGUI coinCountText;

    [System.Serializable]
    public struct FoodCount
    {
        public Image foodIcon;
        public TextMeshProUGUI foodCountText;
    }
    [Header("음식 갯수 UI")]
    public FoodCount foodCount;


    [System.Serializable]
    public struct LeftDownPopUp
    {
        public Animator canvas;
        public TextMeshProUGUI MainText;
        public TextMeshProUGUI ButtonText;
        public bool isShowUp;
    }
    [Header("좌하단 팝업 UI")]
    public LeftDownPopUp PopUpUI;


    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Manager.Day.OnChangeCurDayTime += UpdateDayTimer;
        Manager.Day.OnChangeDay += UpdateDay;
        Manager.Card.OnChangeCardCap += UpdateCardCount;
        Manager.Card.OnChangeCardCount += UpdateCardCount;
        Manager.Card.OnChangeCoinCount += UpdateCoinCount;
        Manager.Card.OnChangeFoodCount += UpdateFoodCount;
        Manager.Card.OnChangeVillagerCount += UpdateFoodCount;
        dayTimer.timerBar.maxValue = Manager.Day.MaxDayTime;


        InitUI();
    }

    public void ShowTopUI()
    {
        topUI.SetBool("Show", true);
    }
    public void HideTopUI()
    {
        topUI.SetBool("Show", false);
    }

    public void UpdateDayTimer()
    {
        dayTimer.timerBar.value = Manager.Day.CurDayTime;
    }

    public void UpdateDay()
    {
        sb.Clear();
        sb.Append($"{Manager.Day.Day}번째 달");
        dayTimer.dayText.SetText(sb);
    }

    public void UpdateCardCount()
    {
        sb.Clear();
        sb.Append($"{Manager.Card.CardCount}/{Manager.Card.CardCap}");
        cardCount.cardCountText.SetText(sb);
        if (Manager.Card.CardCount > Manager.Card.CardCap)
        {
            if (blinkCardCount == null)
            {
                blinkCardCount = StartCoroutine(BlinkCardCount());
            }
        }
        else
        {
            if (blinkCardCount != null)
            {
                StopCoroutine(blinkCardCount);
                blinkCardCount = null;
                cardCount.cardCountText.color = Color.black;
                cardCount.cardIcon.color = Color.black;
            }
        }
    }
    WaitForSeconds second = new WaitForSeconds(0.5f);
    Coroutine blinkCardCount;
    IEnumerator BlinkCardCount()
    {
        while (true)
        {
            cardCount.cardCountText.color = Color.red;
            cardCount.cardIcon.color = Color.red;
            yield return second;
            cardCount.cardCountText.color = Color.black;
            cardCount.cardIcon.color = Color.black;
            yield return second;
        }
    }
    public void UpdateFoodCount()
    {
        sb.Clear();
        sb.Append($"{Manager.Card.FoodCount / 2}/{Manager.Card.VillagerCount}");
        foodCount.foodCountText.SetText(sb);
        if (Manager.Card.FoodCount / 2 < Manager.Card.VillagerCount)
        {
            if(blinkFoodCount == null)
            {
                blinkFoodCount = StartCoroutine(BlickFoodCount());
            }
        }
        else
        {
            if(blinkFoodCount != null)
            {
                StopCoroutine(blinkFoodCount );
                blinkFoodCount = null;
                foodCount.foodIcon.color = Color.black;
                foodCount.foodCountText.color = Color.black;
            }
        }
    }
    Coroutine blinkFoodCount;
    IEnumerator BlickFoodCount()
    {
        while (true)
        {
            foodCount.foodIcon.color = Color.red;
            foodCount.foodCountText.color = Color.red;
            yield return second;
            foodCount.foodIcon.color = Color.black;
            foodCount.foodCountText.color = Color.black;
            yield return second;
        }
    }

    public void UpdateCoinCount()
    {
        sb.Clear();
        sb.Append(Manager.Card.CoinCount);
        coinCountText.SetText(sb);
    }

    public void ShowLeftDownPopUpUI()
    {
        PopUpUI.canvas.SetBool("Show", true);
        PopUpUI.isShowUp = true;
    }
    public void HideLeftDownPopUpUI()
    {
        PopUpUI.canvas.SetBool("Show", false);
        PopUpUI.isShowUp = false;
    }
    public void UpdatePopUpUIMainText(StringBuilder sb)
    {
        PopUpUI.MainText.SetText(sb);
    }
    public void UpdatePopUpUIButtonText(StringBuilder sb)
    {
        PopUpUI.ButtonText.SetText(sb);
    }

    void InitUI()
    {
        UpdateDayTimer();
        UpdateDay();
        UpdateCardCount();
        UpdateCoinCount();
        UpdateFoodCount();
    }
}
