using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Co_TitleScene());
    }

    private  IEnumerator Co_TitleScene()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("00.MainMenu");
    }
}
