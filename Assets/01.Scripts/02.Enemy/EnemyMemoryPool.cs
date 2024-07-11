using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target; // 플레이어
    [SerializeField]
    private GameObject enemySpawnPointPrefab; // 적 등장 위치를 알려주는 프리펩
    [SerializeField]
    private GameObject enemyPrefab; // 생성되는 적 프리펩
    [SerializeField]
    private float enemySpawnTime = 500; // 적 생성 주기
    [SerializeField]
    private float enemySpawnLatency = 1; // 타일 생성 후 적 등장까지 대기시간
    [SerializeField]
    private PotalZone potalZone; // 포탈존 참조
    [SerializeField]
    private int enemySpawnCount = 2; // 동시에 생성되는 적의 숫자

    private MemoryPool spawnPointMemoryPool; // 적 등장 위치를 알려주는 오브젝트 생성, 활성/비활성화 관리
    private MemoryPool enemyMemoryPool; // 적 생성, 활설/비활성화 관리

    private Vector2Int mapSize = new Vector2Int(20, 50); // 맵의 크기
    private int activeEnemyCount = 0; // 현재 활성화된 적의 수
       

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        
        StartCoroutine(SpawnTile());
    }

    private IEnumerator SpawnTile()
    {
        int curNumber = 0;
        int maxNumber = 1;       
        
        yield return new WaitForSeconds(2f);
        while (true)
        {
            // 동시에 enemySpawnCount 숫자만큼 적이 생성되도록 반복문 사용
            for (int i = 0; i < enemySpawnCount; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.45f, mapSize.x * 0.45f), 1,
                                                     Random.Range(-mapSize.y * -0.04f, mapSize.y * 0.4f));

                StartCoroutine("SpawnEnemy", item);
            }

            curNumber++;

            if (curNumber >= maxNumber)
            {
                yield break;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().SetUp(target, this);

        activeEnemyCount++; // 활성화된 적의 수 증가

        // 타일 오브젝트 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
        activeEnemyCount--; // 활성화된 적의 수 감소

        if (activeEnemyCount <= 0)
        {
            potalZone.OpenPotal(); // 모든 적이 죽으면 포탈 열기
        }
    }
}
