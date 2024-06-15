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
    private PlayerController    player;                     // 현재 정보가 출력되는 플레이어

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

    private void Awake()
    {
        SetupWeapon();

        // 메소드가 등록되어 있는 이벤트 클래스의(weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
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
}
