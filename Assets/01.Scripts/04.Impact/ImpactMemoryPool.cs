using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType { Target, Enemy, Boss, Turret}
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;
    private MemoryPool[] memoryPool;

    private void Awake()
    {
        // �ǰ� �̺�Ʈ�� ���� �����̸� �������� memoryPool ����
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
        if (hit.transform.CompareTag("ImpactTarget"))
        {
            OnSpawnImpact(ImpactType.Target, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactBoss"))
        {
            OnSpawnImpact(ImpactType.Boss, hit.point, Quaternion.LookRotation(hit.normal));
        }
       

    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rootation)
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rootation;
        item.GetComponent<Impact>().SetUp(memoryPool[(int)type]);
    }
}
