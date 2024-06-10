using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject mainMenu;         // ���� �޴� â
    public GameObject settingsPopUp;    // ���� â
    public GameObject creditsPopUp;     // ũ���� â

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
                default:
                    break;
            }
        }
    }

    private void StartGame()    // Start ��ư�� ������ �ΰ��� ȭ������ �Ѿ��.
    {
        SceneManager.LoadScene("InGame");
    }
    private void OpenTutorial() //Tutorial ��ư�� ������ Ʃ�丮�� â���� �Ѿ��.
    {
        SceneManager.LoadScene("Tutorial");
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
        }
        if (creditsPopUp.activeSelf)
        {
            creditsPopUp.SetActive(false);
        }
        mainMenu.SetActive(true);
    }
}
