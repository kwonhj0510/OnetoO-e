using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField]
    private Transform turretHead = null;    // ȸ���� �ͷ��� �Ӹ��κ�
    [SerializeField]
    private float range = 5f;               // �ͷ��� �����Ÿ�
    [SerializeField]
    private float idleRotateSpeed = 0f;   // ��ҿ� ȸ���ϴ� �ӵ�
    [SerializeField]
    private float targetRotateSpeed = 0f;   // Ÿ���� ���� �� ȸ���ϴ� �ӵ�
    [SerializeField]
    private LayerMask layerMask;            // Ư�� ���̾ ���� ��� ����
    [SerializeField]
    private bool isTurret0 = true;          // true�̸� turret0, false�̸� turret1

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
    private GameObject muzzleFlashEffect;                 // �ѱ� ����Ʈ (On/Off)

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipFire;        // �߻� �Ҹ�

    private Transform tfTarget = null;      // ������ ���
    private float idleRotationAngle;        // �ͷ��� �¿�� ȸ���� ����
    private float rotationDirection = 1f;   // ȸ�� ����

    private float lastAttackTime = 0;

    private AudioSource audioSource;  // ���� ��� ������Ʈ

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
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
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
            // turret0 �� ���� -180 �������� ������
            idleRotationAngle = -180;
            float rotationAngle = Mathf.Lerp(0, idleRotationAngle, Mathf.PingPong(Time.time * -idleRotateSpeed, 1f));
            turretHead.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
        else
        {
            // turret1 �� ���� 180 �������� ������
            idleRotationAngle = 180;
            float rotationAngle = Mathf.Lerp(0, idleRotationAngle, Mathf.PingPong(Time.time * -idleRotateSpeed, 1f));
            turretHead.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }
    private void LookAtTarget()
    {
        Vector3 targetDirection = tfTarget.position - turretHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);

        // ���� ȸ���� ��ǥ ȸ�� ���̸� ����
        Quaternion targetRotation = Quaternion.Lerp(turretHead.rotation, lookRotation, targetRotateSpeed * Time.deltaTime);

        // �ͷ��� �Ӹ��� ���ο� ȸ�� ������ ����
        turretHead.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);

        // �Ѿ� �߻�
        StartCoroutine("Attack");
        
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        if (Time.time - lastAttackTime > attackRate)
        {
            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // �߻�ü ����
            GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            clone.GetComponent<EnemyProjectile>().SetUp(tfTarget.position);
            // �ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            // �߻� �Ҹ�
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
        audioSource.Stop();                     // ������ ������� ���带 �����ϰ�
        audioSource.clip = clip;                // ���ο� ���� clip���� ��ü ��,
        audioSource.Play();                     // ���� ���
    }
}
