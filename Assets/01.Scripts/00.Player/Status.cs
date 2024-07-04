using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// HP ��ȭ �̺�Ʈ ����
[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    // �ܺο��� �̺�Ʈ�� ����� �� �ֵ��� ����
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed; // �ȱ� �ӵ�
    [SerializeField]
    private float runSpeed;  // �ٱ� �ӵ�

    [Header("HP")]
    [SerializeField]
    private int maxHP = 300; // �ִ� HP
    private int curHP;       // ���� HP
    [SerializeField]
    private Slider HPBar;     // HPBar UI ���

    // �ȱ� �ӵ��� ���� public getter
    public float WalkSpeed => walkSpeed;
    // �ٱ� �ӵ��� ���� public getter
    public float RunSpeed => runSpeed;

    // ���� HP�� ���� public getter
    public int CurHP => curHP;
    // �ִ� HP�� ���� public getter
    public int MaxHP => maxHP;

    private void Awake()
    {
        curHP = maxHP; // ���� HP�� �ִ� HP�� �ʱ�ȭ
        UpdateHPBar(); // HPBar�� �ʱ� HP ���·� ������Ʈ
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = curHP; // ���� HP ����

        // �������� ���� ���� HP ���, 0 ���Ϸ� �������� �ʰ� ����
        curHP = curHP - damage > 0 ? curHP - damage : 0;

        // HP ��ȭ �̺�Ʈ ȣ��
        onHPEvent.Invoke(previousHP, curHP);
        // HPBar ������Ʈ
        UpdateHPBar();

        // HP�� 0�� �Ǹ� true ��ȯ
        if (curHP == 0)
        {
            return true;
        }
        // �׷��� ������ false ��ȯ
        return false;
    }

    private void UpdateHPBar()
    {
        // HPBar�� null�� �ƴ� ��쿡�� ������Ʈ
        if (HPBar != null)
        {
            // HPBar�� fillAmount�� ���� HP�� ����ϵ��� ����
            HPBar.value = (float)curHP / maxHP;
        }
    }
}
