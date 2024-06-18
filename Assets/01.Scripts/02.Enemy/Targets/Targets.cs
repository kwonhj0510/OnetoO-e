using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Targets : MonoBehaviour
{
    static public Targets instance;
     
    public int numberOfTargets = 10; // ������ ���� ����
    private int currentTargetCount;   // ���� ������ ���� ����
    public int targetCount = 0;           // �μ� ���� ����
    public bool isSpawningAllowed = true;  // ���� ���� ��� ����

    [SerializeField]
    private GameObject targetPrefab;  // ���� ������
    [SerializeField]
    private GameObject explanationPanel;

    private TargetHealth targetHealth;
    [SerializeField]
    private TextMeshPro textCurScore;
    [SerializeField]
    private TextMeshPro textMaxScore;

    private void Awake()
    {
        instance = this;    // �̱���
        targetHealth.onScoreEvent.AddListener(UpdateScore);

    }

    private void Update()
    {
        if (targetCount >= numberOfTargets)
        {
            isSpawningAllowed = false;  // ��� ������ �����Ǹ� ���� �ߴ�
            StartCoroutine(RemoveTarget());
        }
    }

    public void StartSpawn()
    {
        currentTargetCount = 0;
        isSpawningAllowed = true;
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (currentTargetCount < numberOfTargets)
        {
            if (isSpawningAllowed && !explanationPanel.activeSelf)
            {
                // ������ ��ġ ����
                float randomX = Random.Range(-8f, 8f);
                float randomY = Random.Range(1f, 6f);
                float fixedZ = transform.position.z; // ���� ������Ʈ�� z ��ġ ���

                Vector3 spawnPosition = new Vector3(randomX, randomY, fixedZ);
                Quaternion spawnRotation = Quaternion.Euler(90f, 0f, 0f);

                // Ÿ�� ����
                GameObject newTarget = Instantiate(targetPrefab, spawnPosition, spawnRotation);
                newTarget.transform.SetParent(transform); // �ڽ����� ����

                currentTargetCount++;
                isSpawningAllowed = false; // ���� �� ���� ������ ����
            }

            yield return null;
        }
    }

    public void TargetDestroyed()
    {
        targetCount++;

        // ��� ������ �����Ǿ���, �μ� ���� ������ ��� ���� ������ ��ġ�� ��
        if (currentTargetCount >= numberOfTargets && targetCount >= numberOfTargets)
        {
            StartCoroutine(RemoveTarget());
        }
        else
        {
            isSpawningAllowed = true; // ���� ���� ���� ���
        }
    }

    public IEnumerator RemoveTarget()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("��� ������ �����߽��ϴ�.");
        SceneManager.LoadScene("03.ClearTutorial");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void UpdateScore(int curScore, int maxScore)
    {
        textCurScore.text = $"{curScore}";
        textMaxScore.text = $"{maxScore}";
    }
}
