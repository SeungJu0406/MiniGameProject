using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;

    [System.Serializable]
    public struct TitleUI
    {
        public GameObject UI;
        public bool isShow;
    }
    public TitleUI titleUI;
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
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InputESC();
        }
    }

    void InputESC()
    {
        if (optionUI.isOptineUi)
        {
            HideOptionUI();
            ShowTitleUI();
        }
    }
    public void ExitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }

    void Init()
    {
        Manager.Sound.Mute();

        ShowTitleUI();
        HideOptionUI();
        InitSound();

        StartCoroutine(PlayBGM());
        hideFadeUIRoutine = hideFadeUIRoutine == null ? StartCoroutine(HideFadeUIRoutine()) : hideFadeUIRoutine;

        Manager.Sound.UnMute();
    }




    #region UI
    public void ShowTitleUI()
    {
        titleUI.UI.gameObject.SetActive(true);
        titleUI.isShow = true;
    }
    public void HideTitleUI()
    {
        titleUI.UI.gameObject.SetActive(false);
        titleUI.isShow = false;
    }
    public void ShowOptionUI()
    {
        Manager.Sound.PlaySFX(Manager.Sound.sfx.UIButton);
        optionUI.UI.gameObject.SetActive(true);
        optionUI.isOptineUi = true;
    }
    public void HideOptionUI()
    {
        Manager.Sound.PlaySFX(Manager.Sound.sfx.UIButton);
        optionUI.UI.gameObject.SetActive(false);
        optionUI.isOptineUi = false;
    }
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
        if (hideFadeUIRoutine != null)
        {
            StopCoroutine(hideFadeUIRoutine);
            hideFadeUIRoutine = null;
        }
        fadeUI.gameObject.SetActive(true);
        fadeUI.Play("FadeOut");
    }
    Coroutine hideFadeUIRoutine;
    IEnumerator HideFadeUIRoutine()
    {
        fadeUI.Play("FadeIn");
        yield return new WaitForSeconds(1);
        fadeUI.gameObject.SetActive(false);
        hideFadeUIRoutine = null;
    }
    #endregion

    #region 사운드  
    WaitForSeconds BGMDelay = new WaitForSeconds(5f);
    IEnumerator PlayBGM()
    {
        while (true)
        {
            yield return BGMDelay;
            if (!Manager.Sound.bgmPlayer.isPlaying)
            {
                Manager.Sound.PlayBGM(Manager.Sound.bgm.title);
            }
            yield return null;
        }
    }
    #endregion
}