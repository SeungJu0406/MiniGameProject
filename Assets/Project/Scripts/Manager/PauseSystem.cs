using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale > 0f)
            {
                Pause();
            }
            else
            {
                normal();
            }
        }
    }

    public void Pause()
    {
        Manager.UI.ShowTopPauseButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.HideTopFastButton();
        Manager.Input.CanClick = false;
        Time.timeScale = 0f;
    }

    public void normal()
    {
        Manager.UI.HideTopPauseButton();
        Manager.UI.ShowTopNormalButton();
        Manager.UI.HideTopFastButton();
        Manager.Input.CanClick = true;
        Time.timeScale = 1f;
    }

    public void Fast()
    {
        Manager.UI.HideTopPauseButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.ShowTopFastButton();
        Time.timeScale = 2f;
    }
}
