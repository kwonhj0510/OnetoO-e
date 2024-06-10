using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject mainMenu;         // 메인 메뉴 창
    public GameObject settingsPopUp;    // 설정 창
    public GameObject creditsPopUp;     // 크레딧 창

    public Button[] buttons;

    void Start()
    {
        foreach (Button button in buttons)
        {
            string buttonName = button.gameObject.name; // 버튼의 이름 가져오기

            // 버튼의 이름에 따라 다른 기능 수행
            switch (buttonName)
            {
                case "START":
                    button.onClick.AddListener(StartGame);
                    break;
                case "TUTORIAL":
                    button.onClick.AddListener(OpenTutorial);
                    break;
                case "SETTINGS":
                    button.onClick.AddListener(OpenSettings);
                    break;
                case "CREDITS":
                    button.onClick.AddListener(ShowCredits);
                    break;
                case "QUIT":
                    button.onClick.AddListener(QuitGame);
                    break;
                case "CLOSE":
                    button.onClick.AddListener(CloseWindow);
                    break;
                default:
                    break;
            }
        }
    }

    private void StartGame()    // Start 버튼을 누르면 인게임 화면으로 넘어간다.
    {
        SceneManager.LoadScene("InGame");
    }
    private void OpenTutorial() //Tutorial 버튼을 누르면 튜토리얼 창으로 넘어간다.
    {
        SceneManager.LoadScene("Tutorial");
    }
    private void OpenSettings() // Settings 버튼을 누르면 설정 창으로 넘어간다.
    {
        mainMenu.SetActive(false);
        settingsPopUp.SetActive(true);
    }
    private void ShowCredits()  // Credits 버튼을 누르면 크레딧 창으로 넘어간다.
    {
        mainMenu.SetActive(false);
        creditsPopUp.SetActive(true);
    }
    private void QuitGame()     // Quit 버튼을 누르면 게임이 종료된다.
    {
        Application.Quit();
    }
    private void CloseWindow()  // Close 버튼을 ( X ) 누르면 창이 닫히고 메인 화면으로 돌아간다.
    {
        if (settingsPopUp.activeSelf)
        {
            settingsPopUp.SetActive(false);
        }
        if (creditsPopUp.activeSelf)
        {
            creditsPopUp.SetActive(false);
        }
        mainMenu.SetActive(true);
    }
}
