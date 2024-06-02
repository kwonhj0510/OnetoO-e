using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { AssaultRifle = 0 };

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;       // 무기 이름
    public int currentAmmo;             // 현재 탄환 수
    public int maxAmmo;                 // 최대 탄환 수
    public int currentMaxAmmo;          // 현재 탄환 수의 최대
    public int reloadAmount;            // 장전되는 탄환 수
    public float attackRate;            // 공격 속도
    public float attackDistance;        // 공격 사거리
    public bool isAutomaticAttack;      // 연속 공격 여부
}
