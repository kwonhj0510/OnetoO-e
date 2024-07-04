using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8; // 인식 범위
    [SerializeField]
    private float pursuitLimitRange = 10;   //추적 범위

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float attackRate = 1;
    

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip audioClipFire;  // 총 쏘는 사운드

    private EnemyState enemyState = EnemyState.None; // 현재 적 행동
    private float lastAttackTime = 0;

    private Status status;  // 이동속도 등의 정보
    private NavMeshAgent navMeshAgent;
    private Transform target;   // 적의 공격 대상 (플레이어)
    private EnemyMemoryPool enemyMemoryPool;
    private PotalZone potalZone;
    private AudioSource audioSource;  // 사운드 재생 컴포넌트


    public void SetUp(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        potalZone = GetComponent<PotalZone>();
        audioSource = GetComponent<AudioSource>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // 적이 활성화 될 때 적의 상태를 "대기"로 설정
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();                     // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip;                // 새로운 사운드 clip으로 교체 후,
        audioSource.Play();                     // 사운드 재생
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (enemyState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        // 현재 적의 상태를 newState로 설정
        enemyState = newState;
        // 새로운 상태 재생
        StartCoroutine(enemyState.ToString());
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

    private IEnumerator ChangeToWander()
    {
        // 1~4초 대기
        int changeTime = UnityEngine.Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;

        // 목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while(true)
        {
            curTime += Time.deltaTime;

            // 목표위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있으면
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || curTime >= maxTime)
            {
                // 상태를 "대기"로 변경
                ChangeState(EnemyState.Idle);
            }
            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;   // 선택된 각도 (wanderJitterMin-wanderJitterMax)
        int wanderJitterMin = 0;    //최소 각도
        int wanderJitterMax = 360;  //최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기 (구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100f;

        // 자신의 위치를 중심으로 반지름 거리, 선택된 각도에 위치한 좌표를 목표지점으로 설정
        wanderJitter = UnityEngine.Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
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
            // 이동 속도 설정
            navMeshAgent.speed = status.RunSpeed;

            // 목표위치를 현재 플레이어의 위치로 설정
            navMeshAgent.SetDestination(target.position);

            // 타겟 방향을 계속 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Attack()
    {
        // 공격을 할 때는 이동을 멈추도록 설정
        navMeshAgent.ResetPath();

        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();
            
            if(Time.time - lastAttackTime > attackRate)
            {
                // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
                lastAttackTime = Time.time;

                // 발사체 생성
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().SetUp(target.position);

                PlaySound(audioClipFire);
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
        transform.rotation = Quaternion.LookRotation(to-from);
        // 서서히 돌기
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        // 플레이어와 적의 거리 계산 후 거리에 따라 행동 선택
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
        //"배회" 상태일 때 이동할 경로 표시
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination-transform.position);

        // 목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 목표 추적 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 공격 범위
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
