using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class PotalZone : MonoBehaviour
{
    [SerializeField]
    private GameObject potalDestination; // ��Ż�� ������
    [SerializeField]
    private ParticleSystem potalParticle;   // ��Ż ��ƼŬ
    [SerializeField]
    private Collider potalCollider; // ��Ż�� ������ �ݶ��̴�
    [SerializeField]
    private float fadeSpeed = 1.0f; // ��Ż�� ������ �ӵ�
    private MeshRenderer meshRenderer;
    private Color originalColor; // ���� ����
    private Color transparentColor; // ������ ����

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // MeshRenderer ������Ʈ ��������
        originalColor = meshRenderer.material.color; // ���� ���� ����
        transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // ������ ���� ����

    }

    private void Start()
    {
        potalCollider.isTrigger = false; // �ʱ⿡�� ��Ż�� �ݶ��̴��� ��Ȱ��ȭ
        meshRenderer.material.color = transparentColor; // ó���� �����ϰ� ����
        potalParticle.gameObject.SetActive(false);
    }

    public void OpenPotal()
    {
        potalCollider.isTrigger = true; // ��Ż�� �ݶ��̴��� Ȱ��ȭ
        StartCoroutine("FadeIn"); // ���̵� �� �ڷ�ƾ ����
        potalParticle.gameObject.SetActive(true);
    }

    private IEnumerator FadeIn()
    {
        float progress = 0;

        // ���̵� �� ȿ���� ���� �ݺ���
        while (progress < 1)
        {
            progress += fadeSpeed * Time.deltaTime; // ���൵ ����
            meshRenderer.material.color = Color.Lerp(transparentColor, originalColor, progress); // ���� ����
            yield return null; // ���� �����ӱ��� ���
        }

        meshRenderer.material.color = originalColor; // ���������� ���� �������� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ��Ż�� ���Դ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // ���� �� �̸��� �����ɴϴ�.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // ���� �� �̸��� ���� �ٸ� ���� �ε��մϴ�.
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
                // ������ ���������� ���, Ŀ���� ���̰� �ϰ� ���� ������ �� Ŭ���� ���� �ε��մϴ�.
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("06.Clear");
            }
        }
    }
}
