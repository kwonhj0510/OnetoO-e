using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;   // �޸��� Ű

    private RotateToMouse   rotateToMouse;              // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovement  movement;                   // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status          status;                     // �̵��ӵ� ���� �÷��̾� ����
           
    void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ��ϰ� ���� ��ġ�� ������Ų��.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;
                
        rotateToMouse       = GetComponent<RotateToMouse>();
        movement            = GetComponent<PlayerMovement>();
        status              = GetComponent<Status>();
    }

    void Update()
    {
        UpdateRotate();
        UpdateMove();
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
       if( x != 0 || z != 0 )
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����.
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
        }
        movement.MoveTo(new Vector3(x, 0, z));
    }
}
