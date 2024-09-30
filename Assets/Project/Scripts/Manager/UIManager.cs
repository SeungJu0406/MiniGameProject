using System.Collections;
using System.Collections.Generic;
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

    [Header("갯수 등 필드 정보 UI")]
    public TextMeshProUGUI cardCountText;


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
        dayTimer.timerBar.maxValue = Manager.Card.MaxDayTime;

        InitUI();
    }
    void InitUI()
    {
        UpdateDayTimer();
        UpdateDay();
        UpdateCardCount();
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
        cardCountText.SetText(sb);
    }
}
