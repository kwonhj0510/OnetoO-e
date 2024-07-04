using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField]
    private Transform turretHead = null;    // 회전할 터렛의 머리부분
    [SerializeField]
    private float range = 5f;               // 터렛의 사정거리
    [SerializeField]
    private float idleRotateSpeed = 0f;   // 평소에 회전하는 속도
    [SerializeField]
    private float targetRotateSpeed = 0f;   // 타겟을 향할 때 회전하는 속도
    [SerializeField]
    private LayerMask layerMask;            // 특정 레이어를 가진 대상만 검출
    [SerializeField]
    private bool isTurret0 = true;          // true이면 turret0, false이면 turret1

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 0.4f;
    [SerializeField]
    private float attackRate = 1;
    [SerializeField]
    private GameObject muzzleFlashEffect;                 // 총구 이펙트 (On/Off)

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipFire;        // 발사 소리

    private Transform tfTarget = null;      // 공격할 대상
    private float idleRotationAngle;        // 터렛이 좌우로 회전할 각도
    private float rotationDirection = 1f;   // 회전 방향

    private float lastAttackTime = 0;

    private AudioSource audioSource;  // 사운드 재생 컴포넌트

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        InvokeRepeating("SearchTarget", 0f, 0.5f);
    }

    private void Update()
    {
        if (tfTarget == null)
        {
            Idle();
        }
        else
        {
            LookAtTarget();
        }
    }

    private void OnEnable()
    {
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
    }

    private void SearchTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, range, layerMask);
        Transform shortestTarget = null;

        if (cols.Length > 0)
        {
            float shortestDistance = Mathf.Infinity;
            foreach (Collider coltarget in cols)
            {
                float distance = Vector3.SqrMagnitude(transform.position - coltarget.transform.position);
                if (shortestDistance > distance)
                {
                    shortestDistance = distance;
                    shortestTarget = coltarget.transform;
                }
            }

        }
        tfTarget = shortestTarget;
    }
    private void Idle()
    {
        if (isTurret0)
        {
            // turret0 일 때는 -180 간격으로 움직임
            idleRotationAngle = -180;
            float rotationAngle = Mathf.Lerp(0, idleRotationAngle, Mathf.PingPong(Time.time * -idleRotateSpeed, 1f));
            turretHead.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
        else
        {
            // turret1 일 때는 180 간격으로 움직임
            idleRotationAngle = 180;
            float rotationAngle = Mathf.Lerp(0, idleRotationAngle, Mathf.PingPong(Time.time * -idleRotateSpeed, 1f));
            turretHead.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }
    private void LookAtTarget()
    {
        Vector3 targetDirection = tfTarget.position - turretHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);

        // 현재 회전과 목표 회전 사이를 보간
        Quaternion targetRotation = Quaternion.Lerp(turretHead.rotation, lookRotation, targetRotateSpeed * Time.deltaTime);

        // 터렛의 머리를 새로운 회전 값으로 설정
        turretHead.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);

        // 총알 발사
        StartCoroutine("Attack");
        
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        if (Time.time - lastAttackTime > attackRate)
        {
            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;

            // 발사체 생성
            GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            clone.GetComponent<EnemyProjectile>().SetUp(tfTarget.position);
            // 총구 이펙트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            // 발사 소리
            PlaySound(audioClipFire);
        }

    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        muzzleFlashEffect.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();                     // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip;                // 새로운 사운드 clip으로 교체 후,
        audioSource.Play();                     // 사운드 재생
    }
}
