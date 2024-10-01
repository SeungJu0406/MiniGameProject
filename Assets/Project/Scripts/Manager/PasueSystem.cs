using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasueSystem : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale > 0f)
            {
                Pasue();
            }
            else
            {
                normal();
            }
        }
    }

    public void Pasue()
    {
        Manager.UI.ShowTopPasueButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.HideTopFastButton();
        Manager.Input.CanClick = false;
        Time.timeScale = 0f;
    }

    public void normal()
    {
        Manager.UI.HideTopPasueButton();
        Manager.UI.ShowTopNormalButton();
        Manager.UI.HideTopFastButton();
        Manager.Input.CanClick = true;
        Time.timeScale = 1f;
    }

    public void Fast()
    {
        Manager.UI.HideTopPasueButton();
        Manager.UI.HideTopNormalButton();
        Manager.UI.ShowTopFastButton();
        Time.timeScale = 2f;
    }
}
