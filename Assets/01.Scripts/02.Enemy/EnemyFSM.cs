using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8; // �ν� ����
    [SerializeField]
    private float pursuitLimitRange = 10;   //���� ����

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float atttackRate = 1;

    private EnemyState enemyState = EnemyState.None; // ���� �� �ൿ
    private float lastAttackTime = 0;

    private Status status;  // �̵��ӵ� ���� ����
    private NavMeshAgent navMeshAgent;
    private Transform target;   // ���� ���� ��� (�÷��̾�)
    private EnemyMemoryPool enemyMemoryPool;
    private PotalZone potalZone;

    //private void Awake()
    public void SetUp(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        potalZone = GetComponent<PotalZone>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ �� �� ���� ���¸� "���"�� ����
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ���� ������ return
        if (enemyState == newState) return;

        // ������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        // ���� ���� ���¸� newState�� ����
        enemyState = newState;
        // ���ο� ���� ���
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n�� �� "��ȸ" ���·� ����Ǵ� �ڷ�ƾ ����
        StartCoroutine("ChangeToWander");

        while (true)
        {
            // "���" ������ ���ϴ� �ൿ
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }

    }

    private IEnumerator ChangeToWander()
    {
        // 1~4�� ���
        int changeTime = UnityEngine.Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // �̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        // ��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ��
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while(true)
        {
            curTime += Time.deltaTime;

            // ��ǥ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || curTime >= maxTime)
            {
                // ���¸� "���"�� ����
                ChangeState(EnemyState.Idle);
            }
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0;   // ���õ� ���� (wanderJitterMin-wanderJitterMax)
        int wanderJitterMin = 0;    //�ּ� ����
        int wanderJitterMax = 360;  //�ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ�� (������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100f;

        // �ڽ��� ��ġ�� �߽����� ������ �Ÿ�, ���õ� ������ ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = UnityEngine.Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x-rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = MathF.Cos(angle) * radius;
        position.z = MathF.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        while (true)
        {
            // �̵� �ӵ� ����
            navMeshAgent.speed = status.RunSpeed;

            // ��ǥ��ġ�� ���� �÷��̾��� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            // Ÿ�� ������ ��� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Attack()
    {
        // ������ �� ���� �̵��� ���ߵ��� ����
        navMeshAgent.ResetPath();

        while (true)
        {
            // Ÿ�� ���� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();
            
            if(Time.time - lastAttackTime > atttackRate)
            {
                // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
                lastAttackTime = Time.time;

                // �߻�ü ����
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().SetUp(target.position);
            }

            yield return null;
        }
    }


    private void LookRotationToTarget()
    {
        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // �� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // �ٷ� ����
        transform.rotation = Quaternion.LookRotation(to-from);
        // ������ ����
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // �÷��̾�� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if(distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= pursuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private void OnDrawGizmos()
    {
        //"��ȸ" ������ �� �̵��� ��� ǥ��
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination-transform.position);

        // ��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ��ǥ ���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }
    }
}
