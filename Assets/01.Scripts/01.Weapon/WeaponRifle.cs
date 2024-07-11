using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class WeaponRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    public LayerMask ignoreLayer; // 무시하는 레이어

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;                 // 총구 이펙트 (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           bulletSpawnPoint;                  // 총알 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           audioClipFire;                     // 공격 사운드
    [SerializeField]
    private AudioClip           audioClipReload;                   // 재장전 사운드

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting       weaponSetting;                     // 무기 설정
    [SerializeField]
    private GameObject          bulletLaser;                       // 총알(투사체)

    private float               lastAttackTime = 0;                // 마지막 발사시간 체크용
    private bool                isReload = false;                  // 재장전 중인지 체크

    private AudioSource                 audioSource;               // 사운드 재생 컴포넌트
    private PlayerAnimatorController    animator;                  // 애니메이션 재생 제어
    private ImpactMemoryPool            impactMemoryPool;          // 공격 효과 생성 후 활성/비활성 관리
    private Camera                      mainCamera;                // 광선 발사

    [SerializeField]
    private Animator bulletLaserAnimator;

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.WeaponName;

    private void Awake()
    {
        audioSource         = GetComponent<AudioSource>();
        animator            = GetComponentInParent<PlayerAnimatorController>();
        impactMemoryPool    = GetComponent<ImpactMemoryPool>();
        mainCamera          = Camera.main;

        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.currentMaxAmmo;
    }

    private void OnEnable()
    {
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);

        // 무기가 활성화 될 때 해당 무기의 탄 수의 정보를 갱신한다.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();                     // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip;                // 새로운 사운드 clip으로 교체 후,
        audioSource.Play();                     // 사운드 재생
    }

    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다.
        if (isReload == true) return;

        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if (isReload == true) return;

        // 무기 액션 도중에 'R'키를 누르면 무기 액션이 종료 후 재장전
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
            // 뛰고있을 때는 공격할 수 없다
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 설정
            lastAttackTime = Time.time;

            // 탄 수가 없으면 자동으로 재장전
            if (weaponSetting.currentAmmo <= 0)
            {
                StartReload();

                return;
            }
            // 공격 시 currentAmmo 1 감소, 탄 수 UI 업데이트
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // 무기 애니메이션 재생
            animator.Play("Fire", -1, 0);
            // 총알(투사체) 발사
            bulletLaserAnimator.SetTrigger("Attack");
            // 총구 이펙트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            SoundManager.instance.PlaySFX("Shoot");


            // 광선을 발사해 원하는 위치 공격 (+Impact Effect)
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

            // 재장전 애니메이션, 사운드
            animator.OnReload();
            SoundManager.instance.PlaySFX("Reload");

            while (true)
            {
                // 사운드가 재생 중이 아니고, 현재 애니메이션이 Movement이면
                // 재장전 애니메이션, 사운드 재생이 종료 되었다는 뜻
                if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
                {
                    isReload = false;

                    // 장전되는 탄환 수 계산
                    weaponSetting.reloadAmount = Mathf.Min(weaponSetting.currentMaxAmmo - weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                    // 현재 탄환 수에 장전되는 탄환 수를 더함
                    weaponSetting.currentAmmo += weaponSetting.reloadAmount;

                    // 최대 탄환 수에서 장전되는 탄환 수를 뺌
                    weaponSetting.maxAmmo -= weaponSetting.reloadAmount;

                    // 바뀐 탄환 수 정보를 Text UI에 업데이트
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

        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // 공격 사거리 (attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if ( Physics.Raycast(ray, out hit, weaponSetting.attackDistance, ~ignoreLayer))
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // 첫번째 Raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고, 
        // 총구를 시작지점으로 하여 Raycast 연산
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
