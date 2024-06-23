using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 300;
    private int curHP;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public int CurHP => curHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        curHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = curHP;

        curHP = curHP - damage > 0 ? curHP - damage : 0;

        onHPEvent.Invoke(previousHP, curHP);

        if(curHP == 0) 
        {
            return true;
        }
        return false;
    }
}
