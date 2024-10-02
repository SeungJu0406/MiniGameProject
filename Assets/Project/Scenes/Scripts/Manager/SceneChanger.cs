using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangeScene(string scene)
    {
       Time.timeScale = 1f;

       AsyncOperation oper = SceneManager.LoadSceneAsync(scene);

       StartCoroutine(ChangeSceneRoutine(oper));
    }
    WaitForSecondsRealtime delay = new WaitForSecondsRealtime(2f);
    IEnumerator ChangeSceneRoutine(AsyncOperation oper)
    {
        oper.allowSceneActivation = false;
        if (Manager.Title != null)
        {
            Manager.Title.ShowFadeUI();
        }
        else if (Manager.Game != null)
        {
            Debug.Log("∞‘¿”æ¿ ∆‰¿ÃµÂæ∆øÙ");
            Manager.UI.ShowFadeUI();
        }       
        yield return delay;
        oper.allowSceneActivation = true;
    }
}
