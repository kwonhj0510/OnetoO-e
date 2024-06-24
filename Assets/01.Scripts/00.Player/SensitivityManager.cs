using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager instance;

    public Slider sensitivitySlider;
    private float sensitivityMin = 1f;
    private float sensitivityMax = 3f;
    public float sensitivity = 3f;

    private void Awake()
    {
        // SensitivityManager�� �ν��Ͻ��� �ϳ��� �����ϵ��� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �����̴� �ʱ� ���� �� �� ����
        sensitivitySlider.minValue = sensitivityMin;
        sensitivitySlider.maxValue = sensitivityMax;
        sensitivitySlider.value = sensitivity;

        // �� ������ ó���ϱ� ���� ������ �߰�
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnDestroy()
    {
        //// �޸� ������ �����ϱ� ���� ��ü�� �ı��� �� ������ ����
        //sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
    }

    public void OnSensitivityChanged(float newSensitivity)
    {
        sensitivity = newSensitivity;
        // ���� �������� �÷��̾��� ���콺 ������ ������Ʈ
        Debug.Log($"������ ����Ǿ����ϴ�: {sensitivity}");
    }
}
