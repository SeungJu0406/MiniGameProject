using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public struct TopUI
    {
        public Animator UI;
        public bool isShow;
    }
    [Header("»ó´Ü UI")]
    public TopUI topUI;

    [System.Serializable]
    public struct LeftUI
    {
        public Animator UI;
        public Button button;
        public bool isShow;
    }
    [Header("ÁÂÃø UI")]
    public LeftUI leftUI;
    [System.Serializable]
    public struct DayTimer
    {
        public Slider timerBar;
        public TextMeshProUGUI dayText;
    }
    [Header("»ó´Ü Å¸ÀÌ¸Ó UI")]
    public DayTimer dayTimer;
    [System.Serializable]
    public struct CardCount
    {
        public Image cardIcon;
        public TextMeshProUGUI cardCountText;
    }
    [Header("Ä«µå °¹¼ö UI")]
    public CardCount cardCount;

    [Header("ÄÚÀÎ °¹¼ö UI")]
    public TextMeshProUGUI coinCountText;

    [System.Serializable]
    public struct FoodCount
    {
        public Image foodIcon;
        public TextMeshProUGUI foodCountText;
    }
    [Header("À½½Ä °¹¼ö UI")]
    public FoodCount foodCount;


    [System.Serializable]
    public struct PopUpUI
    {
        public Animator canvas;
        public TextMeshProUGUI MainText;
        public TextMeshProUGUI ButtonText;
        public bool isShow;
    }
    [Header("ÁÂÇÏ´Ü ÆË¾÷ UI")]
    public PopUpUI popUpUI;

    [System.Serializable]
    public struct RecipeUI
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI recipeText;
    }
    [Header("Á¶ÇÕ¹ý UI")]
    public RecipeUI recipeUI;


    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Manager.Day.OnChangeMaxDayTime += UpdateMaxDayTime;
        Manager.Day.OnChangeCurDayTime += UpdateDayTimer;
        Manager.Day.OnChangeDay += UpdateDay;
        Manager.Card.OnChangeCardCap += UpdateCardCount;
        Manager.Card.OnChangeCardCount += UpdateCardCount;
        Manager.Card.OnChangeCoinCount += UpdateCoinCount;
        Manager.Card.OnChangeFoodCount += UpdateFoodCount;
        Manager.Card.OnChangeVillagerCount += UpdateFoodCount;


        InitUI();
    }

    public void ShowTopUI()
    {
        topUI.UI.SetBool("Show", true);
        topUI.isShow = true;
    }
    public void HideTopUI()
    {
        topUI.UI.SetBool("Show", false);
        topUI.isShow = false;
    }
    public void UpdateMaxDayTime()
    {
        dayTimer.timerBar.maxValue = Manager.Day.MaxDayTime;
    }
    public void UpdateDayTimer()
    {
        dayTimer.timerBar.value = Manager.Day.CurDayTime;
    }

    public void UpdateDay()
    {
        sb.Clear();
        sb.Append($"{Manager.Day.Day}¹øÂ° ´Þ");
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
            if (blinkFoodCount == null)
            {
                blinkFoodCount = StartCoroutine(BlickFoodCount());
            }
        }
        else
        {
            if (blinkFoodCount != null)
            {
                StopCoroutine(blinkFoodCount);
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
    public void ShowPopUpUI()
    {
        popUpUI.canvas.SetBool("Show", true);
        popUpUI.isShow = true;
    }
    public void HidePopUpUI()
    {
        popUpUI.canvas.SetBool("Show", false);
        popUpUI.isShow = false;
    }
    public void UpdatePopUpUIMainText(StringBuilder sb)
    {
        popUpUI.MainText.SetText(sb);
    }
    public void UpdatePopUpUIButtonText(StringBuilder sb)
    {
        popUpUI.ButtonText.SetText(sb);
    }

    public void ShowOrHideLeftUI()
    {
        Manager.Sound.PlaySFX(Manager.Sound.sfx.recipeUI);
        if (leftUI.isShow)
        {
            leftUI.UI.SetBool("Show", false);
            leftUI.isShow = false;
        }
        else
        {
            leftUI.UI.SetBool("Show", true);
            leftUI.isShow = true;
        }
    }
    public void HideLeftUI()
    {
        leftUI.UI.SetBool("Show", false);
        leftUI.isShow = false;
    }
    public void UpdateRecipeUI(RecipeData data)
    {
        Manager.Sound.PlaySFX(Manager.Sound.sfx.UIButton);
        sb.Clear();
        sb.Append(data.resultItem[0].item.cardName);
        recipeUI.nameText.SetText(sb);
        sb.Clear();
        for(int i = 0; i < data.reqItems.Length; i++)
        {
            if(i == data.reqItems.Length - 1)
            {
                sb.Append($"{data.reqItems[i].item.cardName}X{data.reqItems[i].count}");
            }
            else
            {
                sb.Append($"{data.reqItems[i].item.cardName}X{data.reqItems[i].count}\n");
            }
        }
        recipeUI.recipeText.SetText(sb);
    }


    void InitUI()
    {
        UpdateMaxDayTime();
        UpdateDayTimer();
        UpdateDay();
        UpdateCardCount();
        UpdateCoinCount();
        UpdateFoodCount();
    }
}
