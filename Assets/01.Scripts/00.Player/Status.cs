using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// HP 변화 이벤트 정의
[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    // 외부에서 이벤트를 등록할 수 있도록 공개
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed; // 걷기 속도
    [SerializeField]
    private float runSpeed;  // 뛰기 속도

    [Header("HP")]
    [SerializeField]
    private int maxHP = 300; // 최대 HP
    private int curHP;       // 현재 HP
    [SerializeField]
    private Slider HPBar;     // HPBar UI 요소

    // 걷기 속도에 대한 public getter
    public float WalkSpeed => walkSpeed;
    // 뛰기 속도에 대한 public getter
    public float RunSpeed => runSpeed;

    // 현재 HP에 대한 public getter
    public int CurHP => curHP;
    // 최대 HP에 대한 public getter
    public int MaxHP => maxHP;

    private void Awake()
    {
        curHP = maxHP; // 현재 HP를 최대 HP로 초기화
        UpdateHPBar(); // HPBar를 초기 HP 상태로 업데이트
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = curHP; // 이전 HP 저장

        // 데미지를 받은 후의 HP 계산, 0 이하로 내려가지 않게 조정
        curHP = curHP - damage > 0 ? curHP - damage : 0;

        // HP 변화 이벤트 호출
        onHPEvent.Invoke(previousHP, curHP);
        // HPBar 업데이트
        UpdateHPBar();

        // HP가 0이 되면 true 반환
        if (curHP == 0)
        {
            return true;
        }
        // 그렇지 않으면 false 반환
        return false;
    }

    private void UpdateHPBar()
    {
        // HPBar가 null이 아닌 경우에만 업데이트
        if (HPBar != null)
        {
            // HPBar의 fillAmount를 현재 HP에 비례하도록 설정
            HPBar.value = (float)curHP / maxHP;
        }
    }
}
