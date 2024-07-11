using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target; // �÷��̾�
    [SerializeField]
    private GameObject enemySpawnPointPrefab; // �� ���� ��ġ�� �˷��ִ� ������
    [SerializeField]
    private GameObject enemyPrefab; // �����Ǵ� �� ������
    [SerializeField]
    private float enemySpawnTime = 500; // �� ���� �ֱ�
    [SerializeField]
    private float enemySpawnLatency = 1; // Ÿ�� ���� �� �� ������� ���ð�
    [SerializeField]
    private PotalZone potalZone; // ��Ż�� ����
    [SerializeField]
    private int enemySpawnCount = 2; // ���ÿ� �����Ǵ� ���� ����

    private MemoryPool spawnPointMemoryPool; // �� ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ��/��Ȱ��ȭ ����
    private MemoryPool enemyMemoryPool; // �� ����, Ȱ��/��Ȱ��ȭ ����

    private Vector2Int mapSize = new Vector2Int(20, 50); // ���� ũ��
    private int activeEnemyCount = 0; // ���� Ȱ��ȭ�� ���� ��
       

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
            // ���ÿ� enemySpawnCount ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
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

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().SetUp(target, this);

        activeEnemyCount++; // Ȱ��ȭ�� ���� �� ����

        // Ÿ�� ������Ʈ ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
        activeEnemyCount--; // Ȱ��ȭ�� ���� �� ����

        if (activeEnemyCount <= 0)
        {
            potalZone.OpenPotal(); // ��� ���� ������ ��Ż ����
        }
    }
}
