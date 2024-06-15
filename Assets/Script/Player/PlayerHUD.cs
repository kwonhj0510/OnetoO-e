using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    private WeaponRifle  weapon;                     // ���� ������ ��µǴ� ����
    [SerializeField]
    private PlayerController    player;                     // ���� ������ ��µǴ� �÷��̾�

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI     textWeaponName;             // ���� �̸�
    [SerializeField]
    private Image               imageWeaponIcon;            // ���� ������
    [SerializeField]
    private Image               imageAmmoIcon;              // �Ѿ� ������
    [SerializeField]
    private Sprite[]            spriteWeaponIcons;          // ���� �����ܿ� ���Ǵ� sprite �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textCurrentAmmo;            // ���� ź�� �� ���
    [SerializeField]
    private TextMeshProUGUI     textMaxAmmo;                // �ִ� ź�� �� ���    

    private void Awake()
    {
        SetupWeapon();

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ������(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
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
