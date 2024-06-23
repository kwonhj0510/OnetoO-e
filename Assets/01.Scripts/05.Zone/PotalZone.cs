using UnityEngine.SceneManagement;
using UnityEngine;

public class PotalZone : MonoBehaviour
{
    [SerializeField]
    private GameObject potalDestination; // ��Ż�� ������
    [SerializeField]
    private Collider potalCollider; // ��Ż�� ������ �ݶ��̴�


    private void Awake()
    {
        potalCollider.isTrigger = false;
        potalDestination.SetActive(false);
    }

    public void OpenPotal()
    {
        potalCollider.isTrigger = true;
        potalDestination.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� �� �̸��� �����ɴϴ�.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // ���� ���� ���� "01.Stage1"�̸� "04.Stage2"�� �ε��մϴ�.
            if (currentSceneName == "01.Stage1")
            {
                SceneManager.LoadScene("04.Stage2");
            }
            else if (currentSceneName == "04.Stage2")
            {
                SceneManager.LoadScene("05.Stage3");
            }
            else if (currentSceneName == "05.Stage3")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("06.Clear");
            }
        }
    }
}
