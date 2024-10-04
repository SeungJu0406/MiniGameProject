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
        curState = State.Null;
        Time.timeScale = 1f;
    }
    private void Start()
    {       
        StartCoroutine(BGMRoutine());
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
                index++;
                index = index >= Manager.Sound.bgm.game.Length ? 0 : index;
            }
            yield return null;
        }
    }

    public void CheckDefeat()
    {
        curState = State.Defeat;
        if(Manager.UI.topUI.isShow) Manager.UI.HideTopUI();         
        if(Manager.UI.leftUI.isShow) Manager.UI.HideLeftUI();
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

        defeatDelayRoutine = defeatDelayRoutine ==null ? StartCoroutine(DefeatDelayRoutine()) : defeatDelayRoutine;
    }
    Coroutine defeatDelayRoutine;
    IEnumerator DefeatDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        Manager.Input.CanClick = false;
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
        else if (curState == State.GameOver)
        {
            ChangeTitleScene();
        }

    }
    public void ChangeTitleScene()
    {       
        if (defeatDelayRoutine != null)
        {
            StopCoroutine(defeatDelayRoutine);
            defeatDelayRoutine = null;
        }
        SceneChanger.Instance.ChangeScene("TitleScene");
    }
}
