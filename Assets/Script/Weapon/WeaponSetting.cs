using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { AssaultRifle = 0 };

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;       // ���� �̸�
    public int currentAmmo;             // ���� źȯ ��
    public int maxAmmo;                 // �ִ� źȯ ��
    public int currentMaxAmmo;          // ���� źȯ ���� �ִ�
    public int reloadAmount;            // �����Ǵ� źȯ ��
    public float attackRate;            // ���� �ӵ�
    public float attackDistance;        // ���� ��Ÿ�
    public bool isAutomaticAttack;      // ���� ���� ����
}
