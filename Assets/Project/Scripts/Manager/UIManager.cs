using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [System.Serializable]
    public struct DayTimer
    {
        public Slider timerBar;
        public TextMeshProUGUI dayText;
    }
    [Header("날짜, 시간 UI")]
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
    public struct CardOver
    {
        public GameObject cardOverUI;
        public TextMeshProUGUI cardOverText;
    }
    [Header("카드 초과 UI")]
    public CardOver cardOver;


    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Manager.Card.OnChangeCurDayTime += UpdateDayTimer;
        Manager.Card.OnChangeDay += UpdateDay;
        Manager.Card.OnChangeCardCap += UpdateCardCount;
        Manager.Card.OnChangeCardCount += UpdateCardCount;
        Manager.Card.OnChangeCoinCount += UpdateCoinCount;
        dayTimer.timerBar.maxValue = Manager.Card.MaxDayTime;


        cardOver.cardOverUI.gameObject.SetActive(false);

        InitUI();
    }

    public void UpdateDayTimer()
    {
        dayTimer.timerBar.value = Manager.Card.CurDayTime;
    }

    public void UpdateDay()
    {
        sb.Clear();
        sb.Append($"{Manager.Card.Day}번째 달");
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

    public void UpdateCoinCount()
    {
        sb.Clear();
        sb.Append(Manager.Card.CoinCount);
        coinCountText.SetText(sb);
    }

    public void PrintCardOver()
    {
        cardOver.cardOverUI.gameObject.SetActive(true);
        int overCount = Manager.Card.CardCount - Manager.Card.CardCap;
        sb.Clear();
        sb.Append($"{overCount}장의 카드가 초과되었습니다");
        cardOver.cardOverText.SetText(sb);
    }
    public void UnPrintCardOver()
    {
        cardOver.cardOverUI.gameObject.SetActive(false);
    }

    void InitUI()
    {
        UpdateDayTimer();
        UpdateDay();
        UpdateCardCount();
        UpdateCoinCount();
    }
}
