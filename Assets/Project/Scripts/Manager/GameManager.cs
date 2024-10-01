using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum State { Null,Defeat, GameOver}
    public State curState;
    public event UnityAction OnDefeat;
    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        OnDefeat += GameOver;
        curState = State.Null;
    }
    private void Start()
    {
        StartCoroutine(BGMRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
           OpenOrCloseMenu();
        }
    }

    void OpenOrCloseMenu()
    {
        if (Manager.UI.menuUI.isMenuUi) // 메뉴 열렸을때
        {
            Manager.UI.HideMenuUI();
        }
        else if(Manager.UI.optionUI.isOptineUi)
        {
            Manager.UI.HideOptionUI();
        }
        else
        {
            Manager.UI.ShowMenuUI();
        }
    }


    WaitForSeconds bgmDelay = new WaitForSeconds(20f);
    IEnumerator BGMRoutine()
    {
        int index = 0;
        while (true)
        {
            if (!Manager.Sound.bgmPlayer.isPlaying)
            {
                yield return bgmDelay;
                Manager.Sound.PlayBGM(Manager.Sound.bgm.game[index]);
                index = index >= Manager.Sound.bgm.game.Length ? 0 : index + 1;
            }
            yield return null;
        }
    }

    public void CheckDefeat()
    {
        curState = State.Defeat;

        if (!Manager.UI.popUpUI.isShow)
        {
            Manager.UI.ShowPopUpUI();
        }
        sb.Clear();
        sb.Append("모두 죽었습니다!");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append("게임 종료");
        Manager.UI.UpdatePopUpUIButtonText(sb);                 
    }
    public void Defeat()
    {
        if (curState == State.Defeat)
        {
            if(Manager.UI.popUpUI.isShow) 
                Manager.UI.ShowPopUpUI();
            OnDefeat?.Invoke();
            curState = State.GameOver;
        }
    }
    void GameOver()
    {
        Debug.Log("게임 오버");

    }
}
