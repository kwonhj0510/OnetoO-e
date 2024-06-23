using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TurretState { None = -1, Idle = 0, Attack, Pursuit }
public class Turret : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8; // �ν� ����


    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float atttackRate = 1;

    private TurretState turretState = TurretState.None; // ���� �� �ൿ
    private float lastAttackTime = 0;

    private Status status;  // �̵��ӵ� ���� ����
    private Transform target;   // ���� ���� ��� (�÷��̾�)
    private EnemyMemoryPool enemyMemoryPool;
    private PotalZone potalZone;

    //private void Awake()
    public void SetUp(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        potalZone = GetComponent<PotalZone>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ �� �� ���� ���¸� "���"�� ����
        ChangeState(TurretState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(turretState.ToString());

        turretState = TurretState.None;
    }

    public void ChangeState(TurretState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ���� ������ return
        if (turretState == newState) return;

        // ������ ������̴� ���� ����
        StopCoroutine(turretState.ToString());
        // ���� ���� ���¸� newState�� ����
        turretState = newState;
        // ���ο� ���� ���
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
            // Ÿ�� ������ ��� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Attack()
    {


        while (true)
        {
            // Ÿ�� ���� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > atttackRate)
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
        transform.rotation = Quaternion.LookRotation(to - from);
        // ������ ����
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // �÷��̾�� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            ChangeState(TurretState.Attack);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(TurretState.Pursuit);
        }

    }

    private void OnDrawGizmos()
    {
        // ��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if (isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }
    }
}
