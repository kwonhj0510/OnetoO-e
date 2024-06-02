using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun      = KeyCode.LeftShift;            // �޸��� Ű
    [SerializeField]
    private KeyCode keyCodeJump     = KeyCode.Space;                // ���� Ű 
    [SerializeField]
    private KeyCode keyCodeReload   = KeyCode.R;                    // ������ Ű
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                            // �ȱ� ����
    [SerializeField]
    private AudioClip audioClipRun;                             // �޸��� ����

    private RotateToMouse                   rotateToMouse;      // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovement                  movement;           // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status                          status;             // �̵��ӵ� ���� �÷��̾� ����
    private PlayerAnimatorController        animator;           // �ִϸ��̼� ��� ����
    private AudioSource                     audioSource;        // ���� ��� ����
    private WeaponAssaultRifle              weapon;             // ���⸦ �̿��� ���� ����

    void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ��ϰ� ���� ��ġ�� ������Ų��.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        rotateToMouse       = GetComponent<RotateToMouse>();
        movement            = GetComponent<PlayerMovement>();
        status              = GetComponent<Status>();
        animator            = GetComponent<PlayerAnimatorController>();
        audioSource         = GetComponent<AudioSource>();
        weapon              = GetComponentInChildren<WeaponAssaultRifle>();
    }

    void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);

    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // �̵� ���϶� (�ȱ�, �ٱ�)
        if ( x != 0 || z != 0 )
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����.
            if ( z > 0 ) isRun = Input.GetKey(keyCodeRun);

            movement.MoveSpeed  = isRun == true ? status.RunSpeed : status.WalkSpeed;
            animator.MoveSpeed  = isRun == true ? 1 : 0.5f;
            audioSource.clip    = isRun == true ? audioClipRun : audioClipWalk;

            // ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
            // ��� ���϶��� �ٽ� ������� �ʵ��� isPlaying���� üũ�ؼ� ���
            if ( audioSource.isPlaying == false )
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            // ������ �� ���尡 ������̸� ����
            if ( audioSource.isPlaying == true )
            {
                audioSource.Stop();
            }
        }
        movement.MoveTo(new Vector3(x, 0, z));
    }
    private void UpdateJump()
    {
        if (Input.GetKey(keyCodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }

        if (Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
    }
}