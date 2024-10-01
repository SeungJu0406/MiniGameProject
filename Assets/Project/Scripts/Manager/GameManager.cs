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
    public void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        OnDefeat += GameOver;
        curState = State.Null;
    }

    public void CheckDefeat()
    {
        curState = State.Defeat;

        if (!Manager.UI.PopUpUI.isShowUp)
        {
            Manager.UI.ShowLeftDownPopUpUI();
        }
        sb.Clear();
        sb.Append("��� �׾����ϴ�!");
        Manager.UI.UpdatePopUpUIMainText(sb);
        sb.Clear();
        sb.Append("���� ����");
        Manager.UI.UpdatePopUpUIButtonText(sb);                 
    }
    public void Defeat()
    {
        if (curState == State.Defeat)
        {
            Manager.UI.HideLeftDownPopUpUI();
            OnDefeat?.Invoke();
            curState = State.GameOver;
        }
    }
    void GameOver()
    {
        Debug.Log("���� ����");
    }
}
