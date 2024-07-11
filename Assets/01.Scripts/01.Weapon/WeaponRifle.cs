using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class WeaponRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    public LayerMask ignoreLayer; // �����ϴ� ���̾�

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;                 // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           bulletSpawnPoint;                  // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           audioClipFire;                     // ���� ����
    [SerializeField]
    private AudioClip           audioClipReload;                   // ������ ����

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting       weaponSetting;                     // ���� ����
    [SerializeField]
    private GameObject          bulletLaser;                       // �Ѿ�(����ü)

    private float               lastAttackTime = 0;                // ������ �߻�ð� üũ��
    private bool                isReload = false;                  // ������ ������ üũ

    private AudioSource                 audioSource;               // ���� ��� ������Ʈ
    private PlayerAnimatorController    animator;                  // �ִϸ��̼� ��� ����
    private ImpactMemoryPool            impactMemoryPool;          // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera                      mainCamera;                // ���� �߻�

    [SerializeField]
    private Animator bulletLaserAnimator;

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.WeaponName;

    private void Awake()
    {
        audioSource         = GetComponent<AudioSource>();
        animator            = GetComponentInParent<PlayerAnimatorController>();
        impactMemoryPool    = GetComponent<ImpactMemoryPool>();
        mainCamera          = Camera.main;

        // ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.currentMaxAmmo;
    }

    private void OnEnable()
    {
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ �� �� �ش� ������ ź ���� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();                     // ������ ������� ���带 �����ϰ�
        audioSource.clip = clip;                // ���ο� ���� clip���� ��ü ��,
        audioSource.Play();                     // ���� ���
    }

    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����.
        if (isReload == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // �ܹ� ����
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if (isReload == true) return;

        // ���� �׼� ���߿� 'R'Ű�� ������ ���� �׼��� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();
            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �ٰ����� ���� ������ �� ����
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ �ڵ����� ������
            if (weaponSetting.currentAmmo <= 0)
            {
                StartReload();

                return;
            }
            // ���� �� currentAmmo 1 ����, ź �� UI ������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0);
            // �Ѿ�(����ü) �߻�
            bulletLaserAnimator.SetTrigger("Attack");
            // �ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            SoundManager.instance.PlaySFX("Shoot");


            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            TwoStepRaycast();

        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        if (weaponSetting.maxAmmo > 0 && weaponSetting.currentAmmo != weaponSetting.currentMaxAmmo)
        {
            isReload = true;

            // ������ �ִϸ��̼�, ����
            animator.OnReload();
            SoundManager.instance.PlaySFX("Reload");

            while (true)
            {
                // ���尡 ��� ���� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸�
                // ������ �ִϸ��̼�, ���� ����� ���� �Ǿ��ٴ� ��
                if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
                {
                    isReload = false;

                    // �����Ǵ� źȯ �� ���
                    weaponSetting.reloadAmount = Mathf.Min(weaponSetting.currentMaxAmmo - weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                    // ���� źȯ ���� �����Ǵ� źȯ ���� ����
                    weaponSetting.currentAmmo += weaponSetting.reloadAmount;

                    // �ִ� źȯ ������ �����Ǵ� źȯ ���� ��
                    weaponSetting.maxAmmo -= weaponSetting.reloadAmount;

                    // �ٲ� źȯ �� ������ Text UI�� ������Ʈ
                    onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
                    yield break;
                }
                yield return null;
            }
        }
    }

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ (Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // ���� ��Ÿ� (attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if ( Physics.Raycast(ray, out hit, weaponSetting.attackDistance, ~ignoreLayer))
        {
            targetPoint = hit.point;
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // ù��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�, 
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;        
        if ( Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance, ~ignoreLayer))
        {
            impactMemoryPool.SpawnImpact(hit);

            TargetHealth targetyHealth = hit.collider.GetComponent<TargetHealth>();
            if (targetyHealth != null)
            {
                targetyHealth.TakeDamage(weaponSetting.damage);
            }
            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            if (hit.transform.CompareTag("ImpactBoss"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    
}
