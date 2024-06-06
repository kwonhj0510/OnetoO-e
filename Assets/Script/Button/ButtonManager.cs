using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
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
                case "SETTING":
                    button.onClick.AddListener(OpenSettings);
                    break;
                case "CREDITS":
                    button.onClick.AddListener(ShowCredits);
                    break;
                case "QUIT":
                    button.onClick.AddListener(QuitGame);
                    break;
                default:
                    break;
            }
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }
    private void OpenTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    private void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    private void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
