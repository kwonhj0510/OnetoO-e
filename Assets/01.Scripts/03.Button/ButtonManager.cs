using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                case "RETURN":
                    button.onClick.AddListener(TutorialReturn);
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
    }
    private void OpenTutorial() //Tutorial ��ư�� ������ Ʃ�丮�� â���� �Ѿ��.
    {
        SceneManager.LoadScene("02.Tutorial");

        // ���콺 Ŀ���� ������ �ʰ��ϰ� ���� ��ġ�� ������Ų��.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
    private void TutorialReturn()
    {
        SceneManager.LoadScene("02.Tutorial");
    }
    private void OpenMainMenu()
    {
        SceneManager.LoadScene("00.MainMenu");
    }
}
