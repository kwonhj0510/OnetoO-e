using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float       mouseXSpeed = 5f;     // ī�޶� x�� ȸ���ӵ�
    [SerializeField]
    private float       mouseYSpeed = 3f;     // ī�޶� y�� ȸ���ӵ�

    private float       limitMinX = -80f;     // ī�޶� x�� ȸ�� ���� (�ּ�)
    private float       limitMaxX = 80f;      // ī�޶� x�� ȸ�� ���� (�ִ�)
    private float       eulerAngleX;
    private float       eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * mouseYSpeed;    // ���콺 ��/�� �̵����� ī�޶� y�� ȸ��
        eulerAngleX -= mouseY * mouseXSpeed;    // ���콺 ��/�� �̵����� ī�޶� x�� ȸ��

        // ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
        eulerAngleX = ClamAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    public float ClamAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);

    }
}