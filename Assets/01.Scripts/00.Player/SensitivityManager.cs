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
        // SensitivityManager의 인스턴스가 하나만 존재하도록 보장
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
        // 슬라이더 초기 설정 및 값 설정
        sensitivitySlider.minValue = sensitivityMin;
        sensitivitySlider.maxValue = sensitivityMax;
        sensitivitySlider.value = sensitivity;

        // 값 변경을 처리하기 위한 리스너 추가
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnDestroy()
    {
        //// 메모리 누수를 방지하기 위해 객체가 파괴될 때 리스너 제거
        //sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
    }

    public void OnSensitivityChanged(float newSensitivity)
    {
        sensitivity = newSensitivity;
        // 게임 로직에서 플레이어의 마우스 감도를 업데이트
        Debug.Log($"감도가 변경되었습니다: {sensitivity}");
    }
}
