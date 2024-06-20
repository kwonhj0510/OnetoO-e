using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;         // ���� �޴� â
    [SerializeField]
    private GameObject settingsPopUp;    // ���� â
    [SerializeField]
    private GameObject creditsPopUp;     // ũ���� â
    [SerializeField]
    private GameObject escPopUp;     // ũ���� â

    public Button[] buttons;

    void Start()
    {
        foreach (Button button in buttons)
        {
            string buttonName = button.gameObject.name; // ��ư�� �̸� ��������

            // ��ư�� �̸��� ���� �ٸ� ��� ����
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
                default:
                    break;
            }
        }
    }

    private void StartGame()    // Start ��ư�� ������ �ΰ��� ȭ������ �Ѿ��.
    {
        SceneManager.LoadScene("01.InGame");
        SoundManager.instance.musicSource.Stop();
    }
    private void OpenTutorial() //Tutorial ��ư�� ������ Ʃ�丮�� â���� �Ѿ��.
    {
        
        SceneManager.LoadScene("02.Tutorial");
        SoundManager.instance.musicSource.Stop();
    }
    private void OpenSettings() // Settings ��ư�� ������ ���� â���� �Ѿ��.
    {
        mainMenu.SetActive(false);
        settingsPopUp.SetActive(true);
    }
    private void ShowCredits()  // Credits ��ư�� ������ ũ���� â���� �Ѿ��.
    {
        mainMenu.SetActive(false);
        creditsPopUp.SetActive(true);
    }
    private void QuitGame()     // Quit ��ư�� ������ ������ ����ȴ�.
    {
        Application.Quit();
    }
    private void CloseWindow()  // Close ��ư�� ( X ) ������ â�� ������ ���� ȭ������ ���ư���.
    {
        if (settingsPopUp.activeSelf)
        {
            settingsPopUp.SetActive(false);
            mainMenu.SetActive(true);

        }
        if (creditsPopUp.activeSelf)
        {
            creditsPopUp.SetActive(false);
            mainMenu.SetActive(true);

        }
        if (escPopUp.activeSelf)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            escPopUp.SetActive(false);
            Time.timeScale = 1;
        }
        
    }
    private void TutorialRestart()
    {
        // ���콺 Ŀ���� ������ �ʰ��ϰ� ���� ��ġ�� ������Ų��.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("02.Tutorial");
    }
    private void OpenMainMenu()
    {
        SceneManager.LoadScene("00.MainMenu");
        SoundManager.instance.musicSource.Play();
    }
}
