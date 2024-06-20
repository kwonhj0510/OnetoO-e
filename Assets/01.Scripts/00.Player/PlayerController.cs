using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;        // 이동속도
    private Vector3 moveForce;      // 이동 힘 (x, z와 y축을 별도로 계산해 실제 이동에 적용)

    [SerializeField]
    private float jumpForce;        // 점프 힘
    [SerializeField]
    private float gravity;          // 중력 계수

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun      = KeyCode.LeftShift;            // 달리기 키
    [SerializeField]
    private KeyCode keyCodeJump     = KeyCode.Space;                // 점프 키 
    [SerializeField]
    private KeyCode keyCodeReload   = KeyCode.R;                    // 재장전 키
    [SerializeField]
    private KeyCode keyCodeSit      = KeyCode.LeftControl;          // 앉기 키
    [SerializeField]
    private KeyCode keyCodeEsc      = KeyCode.Backspace;               // 설정 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                                // 걷기 사운드
    [SerializeField]
    private AudioClip audioClipRun;                                 // 달리기 사운드

    [SerializeField]
    private GameObject escPopUp;     

    private CharacterController           characterController;      // 플레이어 이동 제어를 위한 컴포넌트
    private RotateToMouse                 rotateToMouse;            // 마우스 이동으로 카메라 회전
    private Status                        status;                   // 이동속도 등의 플레이어 정보
    private PlayerAnimatorController      animator;                 // 애니메이션 재생 제어
    private AudioSource                   audioSource;              // 사운드 재생 제어
    private WeaponRifle                   weapon;                   // 무기를 이용한 공격 제어
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

        // 마우스 커서를 보이지 않게하고 현재 위치에 고정시킨다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
        

    void Update()
    {
        // 허공에 떠있으면 중력만큼 y축 이동속도 감소
        if (!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }
        // 1초마다 moveForce 속력으로 이동
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

        // 이동 중일때 (걷기, 뛰기)
        if ( x != 0 || z != 0 )
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없다.
            if ( z > 0 ) isRun = Input.GetKey(keyCodeRun);

            MoveSpeed  = isRun == true ? status.RunSpeed : status.WalkSpeed;
            animator.MoveSpeed  = isRun == true ? 1 : 0.5f;
            audioSource.clip    = isRun == true ? audioClipRun : audioClipWalk;

            // 방향키 입력 여부는 매 프레임 확인하기 때문에
            // 재생 중일때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
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

            // 멈췄을 때 사운드가 재생중이면 정지
            if ( audioSource.isPlaying == true )
            {
                audioSource.Stop();
            }
        }
        Moveto(new Vector3(x ,0, z));
    }
    private void Moveto(Vector3 dir)
    {
        // 이동 방향 = 캐릭터 회전 값 * 방향 값
        dir = transform.rotation * new Vector3(dir.x, 0, dir.z);
        // 이동 힘 = 이동 방향 * 속도
        moveForce = new Vector3(dir.x * moveSpeed, moveForce.y, dir.z * moveSpeed);
    }

    private void UpdateJump()
    {
        if (Input.GetKey(keyCodeJump))
        {
            // 플레이어가 바닥에 있을 때만 점프 가능
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
                Debug.LogError("ESC 팝업 UI가 할당되지 않았습니다.");
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