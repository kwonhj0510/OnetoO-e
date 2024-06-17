using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Targets : MonoBehaviour
{
    static public Targets instance;

    [SerializeField]
    private GameObject targetPrefab;  // 과녁 프리팹
    
    private int numberOfTargets = 10; // 생성할 과녁 개수
    private int currentTargetCount;   // 현재 생성된 과녁 개수
    public int targetCount;          // 부순 과녁 개수

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
            // 랜덤한 위치 생성
            float randomX = Random.Range(-8f, 8f);
            float randomY = Random.Range(1f, 6f);
            float fixedZ = transform.position.z; // 현재 오브젝트의 z 위치 사용

            Vector3 spawnPosition = new Vector3(randomX, randomY, fixedZ);

            Quaternion spawnRotation = Quaternion.Euler(90f, 0f, 0f);

            yield return new WaitForSeconds(2f);

            // 타겟 생성
            GameObject newTarget = Instantiate(targetPrefab, spawnPosition, spawnRotation);
            newTarget.transform.SetParent(transform); // 자식으로 설정

            currentTargetCount++;

        }

        Debug.Log("모든 과녁이 생성되었습니다.");
    }

    public IEnumerator RemoveTarget()
    {
        Debug.Log("모든 과녁을 제거했습니다.");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("03.TutorialClear");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
