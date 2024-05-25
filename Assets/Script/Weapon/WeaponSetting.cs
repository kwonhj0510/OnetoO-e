using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { AssaultRifle = 0 };

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;       // ���� �̸�
    public int currentAmmo;             // ���� ź�� ��
    public int maxAmmo;                 // �ִ� ź�� ��
    public int currentMaxAmmo;          // ���� ź�� ���� �ִ�
    public float attackRate;            // ���� �ӵ�
    public float attackDistance;        // ���� ��Ÿ�
    public bool isAutomaticAttack;      // ���� ���� ����
}
