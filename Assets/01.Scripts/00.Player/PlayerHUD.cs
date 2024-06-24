using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    private WeaponRifle  weapon;                     // 현재 정보가 출력되는 무기
    [SerializeField]
    private Status status;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI     textWeaponName;             // 무기 이름
    [SerializeField]
    private Image               imageWeaponIcon;            // 무기 아이콘
    [SerializeField]
    private Image               imageAmmoIcon;              // 총알 아이콘
    [SerializeField]
    private Sprite[]            spriteWeaponIcons;          // 무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textCurrentAmmo;            // 현재 탄약 수 출력
    [SerializeField]
    private TextMeshProUGUI     textMaxAmmo;                // 최대 탄약 수 출력    

    [Header("HP & Blood Screen")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon is not assigned in the inspector!");
        }
        else
        {
            SetupWeapon();
            weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        }

        if (status == null)
        {
            Debug.LogError("Status is not assigned in the inspector!");
        }
        else
        {
            status.onHPEvent.AddListener(UpdateHPHUD);
        }
    }


    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textCurrentAmmo.text = $"<size=50>{currentAmmo}</size>";
        textMaxAmmo.text = $"{maxAmmo}";
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = $"{current}";

        if (previous - current > 0 )
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }

    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        float maxAlpha = 47f / 255f;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(maxAlpha, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
