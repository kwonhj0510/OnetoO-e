using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;         // 메인 메뉴 창
    [SerializeField]
    private GameObject settingsPopUp;    // 설정 창
    [SerializeField]
    private GameObject creditsPopUp;     // 크레딧 창
    [SerializeField]
    private GameObject escPopUp;         // Esc 창

    [SerializeField] PlayerController playerController;


    public Button[] buttons;

    private void Awake()
    {
    }

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
                case "RESTART":
                    button.onClick.AddListener(TutorialRestart);
                    break;
                case "MENU":
                    button.onClick.AddListener(OpenMainMenu);
                    break;
                case "GAMERESTART":
                    button.onClick.AddListener(StartGame);
                    break;
                default:
                    break;
            }
        }
    }

    private void StartGame()    // Start 버튼을 누르면 인게임 화면으로 넘어간다.
    {
        // 마우스 커서를 보이지 않게하고 현재 위치에 고정시킨다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("01.Stage1");
        SoundManager.instance.musicSource.Stop();
    }
    private void OpenTutorial() //Tutorial 버튼을 누르면 튜토리얼 창으로 넘어간다.
    {
        
        SceneManager.LoadScene("02.Tutorial");
        SoundManager.instance.musicSource.Stop();
    }
    private void OpenSettings() // Settings 버튼을 누르면 설정 창으로 넘어간다.
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "00.MainMenu")
        {
            mainMenu.SetActive(false);
        }
        
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
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (settingsPopUp.activeSelf)
        {
            if (currentSceneName == "00.MainMenu")
            {
                mainMenu.SetActive(true);
                settingsPopUp.SetActive(false);
            }
        }
        
        if (currentSceneName == "00.MainMenu")
        {
            if (creditsPopUp.activeSelf)
            {
                creditsPopUp.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
    }
    private void TutorialRestart()
    {
        // 마우스 커서를 보이지 않게하고 현재 위치에 고정시킨다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("02.Tutorial");
    }
    private void OpenMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if(Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene("00.MainMenu");
        SoundManager.instance.musicSource.Play();
    }
    public void TimeScale()
    {
        playerController.isGameStart = true;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
