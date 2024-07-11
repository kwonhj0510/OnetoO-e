using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public static FadeScript instance;

    [SerializeField] private Image image;
    private Color tmpColor;
    private float fadeDuration = 2f; // ���̵� ���� �ð� (��)

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        tmpColor = Color.black;
        tmpColor.a = 1f; // �ʱ� ���� �� ���� (���� ������)
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
            yield return null; // �� ������ ��ٸ�
        }

        // ���� ���̵��� �� ���� ����
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
            yield return null; // �� ������ ��ٸ�
        }

        // ���� ���̵�ƿ� �� ���� ����
        tmpColor.a = 1f;
        image.color = tmpColor;
    }
}
