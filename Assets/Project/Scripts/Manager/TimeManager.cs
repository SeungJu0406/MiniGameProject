using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private float _timeScale = 1;
    public float TimeScale { get { return _timeScale; } set { _timeScale = value; } }
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void PauseDayResult()
    {
        TimeScale = 0;
    }

    public void Pause()
    {
        Manager.UI.ShowTopPauseButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.HideTopFastButton();
        TimeScale = 0;
    }

    public void Normal()
    {
        Manager.UI.HideTopPauseButton();
        Manager.UI.ShowTopNormalButton();
        Manager.UI.HideTopFastButton();
        TimeScale = 1;
    }

    public void Fast()
    {
        Manager.UI.HideTopPauseButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.ShowTopFastButton();
        TimeScale = 2;
    }
}
