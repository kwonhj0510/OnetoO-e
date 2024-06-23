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
    private float targetRecognitionRange = 8; // 인식 범위


    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float atttackRate = 1;

    private TurretState turretState = TurretState.None; // 현재 적 행동
    private float lastAttackTime = 0;

    private Status status;  // 이동속도 등의 정보
    private Transform target;   // 적의 공격 대상 (플레이어)
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
        // 적이 활성화 될 때 적의 상태를 "대기"로 설정
        ChangeState(TurretState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(turretState.ToString());

        turretState = TurretState.None;
    }

    public void ChangeState(TurretState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (turretState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(turretState.ToString());
        // 현재 적의 상태를 newState로 설정
        turretState = newState;
        // 새로운 상태 재생
    }

    private IEnumerator Idle()
    {
        // n초 후 "배회" 상태로 변경되는 코루틴 실행
        StartCoroutine("ChangeToWander");

        while (true)
        {
            // "대기" 상태일 때하는 행동
            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
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
            // 타겟 방향을 계속 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Attack()
    {


        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > atttackRate)
            {
                // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
                lastAttackTime = Time.time;

                // 발사체 생성
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().SetUp(target.position);
            }

            yield return null;
        }
    }


    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // 내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 바로 돌기
        transform.rotation = Quaternion.LookRotation(to - from);
        // 서서히 돌기
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // 플레이어와 적의 거리 계산 후 거리에 따라 행동 선택
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
        // 목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 공격 범위
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
