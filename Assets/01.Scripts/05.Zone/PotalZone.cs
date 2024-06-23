using UnityEngine.SceneManagement;
using UnityEngine;

public class PotalZone : MonoBehaviour
{
    [SerializeField]
    private GameObject potalDestination; // 포탈의 목적지
    [SerializeField]
    private Collider potalCollider; // 포탈의 목적지 콜라이더


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
            // 현재 씬 이름을 가져옵니다.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // 만약 현재 씬이 "01.Stage1"이면 "04.Stage2"로 로드합니다.
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
