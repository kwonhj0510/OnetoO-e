using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float       mouseXSpeed = 5f;     // 카메라 x축 회전속도
    [SerializeField]
    private float       mouseYSpeed = 3f;     // 카메라 y축 회전속도

    private float       limitMinX = -80f;     // 카메라 x축 회전 범위 (최소)
    private float       limitMaxX = 80f;      // 카메라 x축 회전 범위 (최대)
    private float       eulerAngleX;
    private float       eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * mouseYSpeed;    // 마우스 좌/우 이동으로 카메라 y축 회전
        eulerAngleX -= mouseY * mouseXSpeed;    // 마우스 상/하 이동으로 카메라 x축 회전

        // 카메라 x축 회전의 경우 회전 범위를 설정
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