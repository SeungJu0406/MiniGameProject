using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        AsyncOperation oper = SceneManager.LoadSceneAsync("TitleScene");
        oper.allowSceneActivation = false;
        yield return new WaitForSeconds(4f);
        oper.allowSceneActivation = true;       
    }
}
