using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Targets : MonoBehaviour
{
    static public Targets instance;

    [SerializeField]
    private GameObject targetPrefab;  // ���� ������
    
    private int numberOfTargets = 10; // ������ ���� ����
    private int currentTargetCount;   // ���� ������ ���� ����
    public int targetCount;          // �μ� ���� ����

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (targetCount >= 10)
            StartCoroutine(RemoveTarget());
    }
    public void StartSpawn()
    {
        currentTargetCount = 0;

        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (currentTargetCount < numberOfTargets)
        {
            // ������ ��ġ ����
            float randomX = Random.Range(-8f, 8f);
            float randomY = Random.Range(1f, 6f);
            float fixedZ = transform.position.z; // ���� ������Ʈ�� z ��ġ ���

            Vector3 spawnPosition = new Vector3(randomX, randomY, fixedZ);

            Quaternion spawnRotation = Quaternion.Euler(90f, 0f, 0f);

            yield return new WaitForSeconds(2f);

            // Ÿ�� ����
            GameObject newTarget = Instantiate(targetPrefab, spawnPosition, spawnRotation);
            newTarget.transform.SetParent(transform); // �ڽ����� ����

            currentTargetCount++;

        }

        Debug.Log("��� ������ �����Ǿ����ϴ�.");
    }

    public IEnumerator RemoveTarget()
    {
        Debug.Log("��� ������ �����߽��ϴ�.");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("03.TutorialClear");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
