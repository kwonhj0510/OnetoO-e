using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CloseButtonChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText; // 버튼의 텍스트 컴포넌트
    public Color normalTextColor = Color.white; // 기본 텍스트 색상
    public Color highlightedTextColor = Color.gray; // 강조 텍스트 색상
    public Color pressedTextColor = Color.black; // 눌렸을 때 텍스트 색상

    // 마우스가 버튼 위에 있을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightedTextColor;
    }

    // 마우스가 버튼을 떠날 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalTextColor;
    }

    // 버튼이 눌렸을 때 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonText.color = pressedTextColor;
    }

    // 버튼에서 손을 뗐을 때 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        buttonText.color = highlightedTextColor;
    }
}
