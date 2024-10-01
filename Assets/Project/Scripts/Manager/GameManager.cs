using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event UnityAction OnDefeat;

    public void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Defeat()
    {
        Debug.Log("게임 패배");
        OnDefeat?.Invoke();
    }
}
