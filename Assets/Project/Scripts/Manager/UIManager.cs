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
        public Button pause;
        public GameObject pauseFillter;
        public Button normal;
        public Button fast;
        public bool isShow;
    }
    [Header("상단 UI")]
    [SerializeField] public TopUI topUI;

    [System.Serializable]
    public struct LeftUI
    {
        public Animator UI;
        public Button button;
        public bool isShow;
    }
    [Header("좌측 UI")]
    [SerializeField] public LeftUI leftUI;
    [System.Serializable]
    public struct DayTimer
    {
        public Slider timerBar;
        public TextMeshProUGUI dayText;
    }
    [Header("상단 타이머 UI")]
    [SerializeField] public DayTimer dayTimer;
    [System.Serializable]
    public struct CardCount
    {
        public Image cardIcon;
        public TextMeshProUGUI cardCountText;
    }
    [Header("카드 갯수 UI")]
    [SerializeField] public CardCount cardCount;

    [Header("코인 갯수 UI")]
    [SerializeField] public TextMeshProUGUI coinCountText;

    [System.Serializable]
    public struct FoodCount
    {
        public Image foodIcon;
        public TextMeshProUGUI foodCountText;
    }
    [Header("음식 갯수 UI")]
    [SerializeField] public FoodCount foodCount;


    [System.Serializable]
    public struct PopUpUI
    {
        public Animator canvas;
        public TextMeshProUGUI MainText;
        public TextMeshProUGUI ButtonText;
        public bool isShow;
    }
    [Header("좌하단 팝업 UI")]
    [SerializeField] public PopUpUI popUpUI;

    [System.Serializable]
    public struct RecipeUI
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI recipeText;
    }
    [Header("조합법 UI")]
    [SerializeField] public RecipeUI recipeUI;

    [System.Serializable]
    public struct MenuUI
    {
        public GameObject UI;
        public bool isMenuUi;
    }
    [Header("메뉴 UI")]
    [SerializeField] public MenuUI menuUI;
    [System.Serializable]
    public struct OptineUI
    {
        public GameObject UI;
        public Slider BGMVolume;
        public TextMeshProUGUI BGMVolumeText;
        public Slider SFXVolume;
        public TextMeshProUGUI SFXVolumeText;
        public bool isOptineUi;
    }
    [Header("옵션 UI")]
    [SerializeField] public OptineUI optionUI;

    [Header("화면전환 UI")]
    [SerializeField] public Animator fadeUI;


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

    private void OnDisable()
    {
        StopAllCoroutines();
        blinkFoodCount = null;
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
        for (int i = 0; i < data.reqItems.Length; i++)
        {
            if (i == data.reqItems.Length - 1)
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
    #region 버튼 UI On/Off
    public void ShowTopPauseButton()
    {
        topUI.pause.gameObject.SetActive(true);
        topUI.pauseFillter.gameObject.SetActive(true);
    }
    public void HideTopPauseButton() 
    {
        topUI.pause.gameObject.SetActive(false);
        topUI.pauseFillter.gameObject.SetActive(false);
    }
    public void ShowTopNormalButton()
    {
        topUI.normal.gameObject.SetActive(true);
    }
    public void HideTopNormalButton()
    {
        topUI.normal.gameObject.SetActive(false);
    }
    public void ShowTopFastButton()
    {
        topUI.fast.gameObject.SetActive(true);
    }
    public void HideTopFastButton()
    {
        topUI.fast.gameObject.SetActive(false);
    }
    #endregion

    public void ShowMenuUI()
    {
        menuUI.UI.gameObject.SetActive(true);
        menuUI.isMenuUi = true;
    }
    public void HideMenuUI()
    {
        menuUI.UI.gameObject.SetActive(false);
        menuUI.isMenuUi = false;
    }
    public void ShowOptionUI()
    {
        optionUI.UI.gameObject.SetActive(true);
        optionUI.isOptineUi = true;
    }
    public void HideOptionUI()
    {
        optionUI.UI.gameObject.SetActive(false);
        optionUI.isOptineUi = false;
    }

    // 볼륨 UI  
    public void UpdateBGMVolume()
    {
        sb.Clear();
        sb.Append($"{(int)(optionUI.BGMVolume.value * 100)}%");
        optionUI.BGMVolumeText.SetText(sb);
    }
    public void UpdateSFXVolume()
    {
        sb.Clear();
        sb.Append($"{(int)(optionUI.SFXVolume.value * 100)}%");
        optionUI.SFXVolumeText.SetText(sb);
    }
    public void InitSound()
    {
        optionUI.BGMVolume.value = Manager.Sound.bgmPlayer.volume;
        optionUI.SFXVolume.value = Manager.Sound.sfxPlayer.volume;
        UpdateBGMVolume();
        UpdateSFXVolume();
    }
    public void ShowFadeUI()
    {
        fadeUI.gameObject.SetActive(true);
        fadeUI.Play("FadeOut");
    }
    IEnumerator HideFadeUIRoutine()
    {
        fadeUI.Play("FadeIn");
        yield return second;
        fadeUI.gameObject.SetActive(false);
    }
    void InitUI()
    {
        UpdateMaxDayTime();
        UpdateDayTimer();
        UpdateDay();
        UpdateCardCount();
        UpdateCoinCount();
        UpdateFoodCount();
        ShowTopUI();
        HideTopPauseButton(); ShowTopNormalButton(); HideTopFastButton();
        HideLeftUI();
        HidePopUpUI();
        HideMenuUI();
        HideOptionUI();
        InitSound();

        StartCoroutine(HideFadeUIRoutine());
    }
}
