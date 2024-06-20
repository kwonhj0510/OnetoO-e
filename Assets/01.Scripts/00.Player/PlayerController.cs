using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;        // �̵��ӵ�
    private Vector3 moveForce;      // �̵� �� (x, z�� y���� ������ ����� ���� �̵��� ����)

    [SerializeField]
    private float jumpForce;        // ���� ��
    [SerializeField]
    private float gravity;          // �߷� ���

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun      = KeyCode.LeftShift;            // �޸��� Ű
    [SerializeField]
    private KeyCode keyCodeJump     = KeyCode.Space;                // ���� Ű 
    [SerializeField]
    private KeyCode keyCodeReload   = KeyCode.R;                    // ������ Ű
    [SerializeField]
    private KeyCode keyCodeSit      = KeyCode.LeftControl;          // �ɱ� Ű
    [SerializeField]
    private KeyCode keyCodeEsc      = KeyCode.Backspace;               // ���� Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                                // �ȱ� ����
    [SerializeField]
    private AudioClip audioClipRun;                                 // �޸��� ����

    [SerializeField]
    private GameObject escPopUp;     

    private CharacterController           characterController;      // �÷��̾� �̵� ��� ���� ������Ʈ
    private RotateToMouse                 rotateToMouse;            // ���콺 �̵����� ī�޶� ȸ��
    private Status                        status;                   // �̵��ӵ� ���� �÷��̾� ����
    private PlayerAnimatorController      animator;                 // �ִϸ��̼� ��� ����
    private AudioSource                   audioSource;              // ���� ��� ����
    private WeaponRifle                   weapon;                   // ���⸦ �̿��� ���� ����
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    void Awake()
    {        
        characterController = GetComponent<CharacterController>();
        rotateToMouse       = GetComponent<RotateToMouse>();       
        status              = GetComponent<Status>();
        animator            = GetComponent<PlayerAnimatorController>();
        audioSource         = GetComponent<AudioSource>();
        weapon              = GetComponentInChildren<WeaponRifle>();

        // ���콺 Ŀ���� ������ �ʰ��ϰ� ���� ��ġ�� ������Ų��.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
        

    void Update()
    {
        // ����� �������� �߷¸�ŭ y�� �̵��ӵ� ����
        if (!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }
        // 1�ʸ��� moveForce �ӷ����� �̵�
        characterController.Move(moveForce * Time.deltaTime);
                
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateSit();
        UpdateWeaponAction();
        UpdateEsc();
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

            MoveSpeed  = isRun == true ? status.RunSpeed : status.WalkSpeed;
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
            MoveSpeed = 0;
            animator.MoveSpeed = 0;

            // ������ �� ���尡 ������̸� ����
            if ( audioSource.isPlaying == true )
            {
                audioSource.Stop();
            }
        }
        Moveto(new Vector3(x ,0, z));
    }
    private void Moveto(Vector3 dir)
    {
        // �̵� ���� = ĳ���� ȸ�� �� * ���� ��
        dir = transform.rotation * new Vector3(dir.x, 0, dir.z);
        // �̵� �� = �̵� ���� * �ӵ�
        moveForce = new Vector3(dir.x * moveSpeed, moveForce.y, dir.z * moveSpeed);
    }

    private void UpdateJump()
    {
        if (Input.GetKey(keyCodeJump))
        {
            // �÷��̾ �ٴڿ� ���� ���� ���� ����
            if (characterController.isGrounded)
            {
                moveForce.y = jumpForce;
            }
        }
    }

    private void UpdateSit()
    {
        if (Input.GetKey(keyCodeSit))
        {
            if (characterController.isGrounded)
            {
                Camera.main.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            }
        }
        else if (Input.GetKeyUp(keyCodeSit))
        {
            if (characterController.isGrounded)
            {
                Camera.main.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
            }
        }
    }

    private void UpdateEsc()
    {
        if (Input.GetKeyDown(keyCodeEsc))
        {
            Time.timeScale = 0;
            escPopUp.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
                           
            
            if (escPopUp == null)
            {
                Debug.LogError("ESC �˾� UI�� �Ҵ���� �ʾҽ��ϴ�.");
                return;
            }
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