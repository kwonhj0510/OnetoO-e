using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public static FadeScript instance;

    [SerializeField] private Image image;
    private Color tmpColor;
    private float fadeDuration = 2f; // 페이드 지속 시간 (초)

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        tmpColor = Color.black;
        tmpColor.a = 1f; // 초기 알파 값 설정 (완전 불투명)
        image = GetComponent<Image>();
        image.color = tmpColor;
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tmpColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = tmpColor;
            yield return null; // 한 프레임 기다림
        }

        // 완전 페이드인 된 상태 보장
        tmpColor.a = 0f;
        image.color = tmpColor;
    }
    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tmpColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            image.color = tmpColor;
            yield return null; // 한 프레임 기다림
        }

        // 완전 페이드아웃 된 상태 보장
        tmpColor.a = 1f;
        image.color = tmpColor;
    }
}
