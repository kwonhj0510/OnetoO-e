using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class PotalZone : MonoBehaviour
{
    [SerializeField]
    private GameObject potalDestination; // 포탈의 목적지
    [SerializeField]
    private ParticleSystem potalParticle;   // 포탈 파티클
    [SerializeField]
    private Collider potalCollider; // 포탈의 목적지 콜라이더
    [SerializeField]
    private float fadeSpeed = 1.0f; // 포탈이 열리는 속도
    private MeshRenderer meshRenderer;
    private Color originalColor; // 원래 색상
    private Color transparentColor; // 투명한 색상

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // MeshRenderer 컴포넌트 가져오기
        originalColor = meshRenderer.material.color; // 원래 색상 저장
        transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // 투명한 색상 설정

    }

    private void Start()
    {
        potalCollider.isTrigger = false; // 초기에는 포탈의 콜라이더를 비활성화
        meshRenderer.material.color = transparentColor; // 처음에 투명하게 설정
        potalParticle.gameObject.SetActive(false);
    }

    public void OpenPotal()
    {
        potalCollider.isTrigger = true; // 포탈의 콜라이더를 활성화
        StartCoroutine("FadeIn"); // 페이드 인 코루틴 시작
        potalParticle.gameObject.SetActive(true);
    }

    private IEnumerator FadeIn()
    {
        float progress = 0;

        // 페이드 인 효과를 위한 반복문
        while (progress < 1)
        {
            progress += fadeSpeed * Time.deltaTime; // 진행도 증가
            meshRenderer.material.color = Color.Lerp(transparentColor, originalColor, progress); // 색상 보간
            yield return null; // 다음 프레임까지 대기
        }

        meshRenderer.material.color = originalColor; // 최종적으로 원래 색상으로 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 포탈에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            // 현재 씬 이름을 가져옵니다.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // 현재 씬 이름에 따라 다른 씬을 로드합니다.
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
                // 마지막 스테이지의 경우, 커서를 보이게 하고 고정 해제한 후 클리어 씬을 로드합니다.
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("06.Clear");
            }
        }
    }
}
