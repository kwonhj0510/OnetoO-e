using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [Header("Player HP")]
    [SerializeField]
    private float playerHp;    

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

}
