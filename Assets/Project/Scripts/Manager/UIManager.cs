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
    public DayTimer dayTimer;

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
        dayTimer.timerBar.maxValue = Manager.Card.MaxDayTime;
    }

    public void UpdateDayTimer()
    {
        dayTimer.timerBar.value = Manager.Card.CurDayTime;
    }
    
    public void UpdateDay()
    {
        sb.Clear();
        sb.Append($"{Manager.Card.Day}¹øÂ° ´Þ");
        dayTimer.dayText.SetText(sb);
    }
}
