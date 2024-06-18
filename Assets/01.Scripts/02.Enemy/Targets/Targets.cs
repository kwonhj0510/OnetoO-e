using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Targets : MonoBehaviour
{
    static public Targets instance;
     
    public int numberOfTargets = 10; // 생성할 과녁 개수
    private int currentTargetCount;   // 현재 생성된 과녁 개수
    public int targetCount = 0;           // 부순 과녁 개수
    public bool isSpawningAllowed = true;  // 과녁 생성 허용 여부

    [SerializeField]
    private GameObject targetPrefab;  // 과녁 프리팹
    [SerializeField]
    private GameObject explanationPanel;

    private TargetHealth targetHealth;
    [SerializeField]
    private TextMeshPro textCurScore;
    [SerializeField]
    private TextMeshPro textMaxScore;

    private void Awake()
    {
        instance = this;    // 싱글톤
        targetHealth.onScoreEvent.AddListener(UpdateScore);

    }

    private void Update()
    {
        if (targetCount >= numberOfTargets)
        {
            isSpawningAllowed = false;  // 모든 과녁이 생성되면 생성 중단
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
                // 랜덤한 위치 생성
                float randomX = Random.Range(-8f, 8f);
                float randomY = Random.Range(1f, 6f);
                float fixedZ = transform.position.z; // 현재 오브젝트의 z 위치 사용

                Vector3 spawnPosition = new Vector3(randomX, randomY, fixedZ);
                Quaternion spawnRotation = Quaternion.Euler(90f, 0f, 0f);

                // 타겟 생성
                GameObject newTarget = Instantiate(targetPrefab, spawnPosition, spawnRotation);
                newTarget.transform.SetParent(transform); // 자식으로 설정

                currentTargetCount++;
                isSpawningAllowed = false; // 생성 후 다음 생성을 막음
            }

            yield return null;
        }
    }

    public void TargetDestroyed()
    {
        targetCount++;

        // 모든 과녁이 생성되었고, 부순 과녁 개수가 모든 과녁 개수와 일치할 때
        if (currentTargetCount >= numberOfTargets && targetCount >= numberOfTargets)
        {
            StartCoroutine(RemoveTarget());
        }
        else
        {
            isSpawningAllowed = true; // 다음 과녁 생성 허용
        }
    }

    public IEnumerator RemoveTarget()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("모든 과녁을 제거했습니다.");
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
