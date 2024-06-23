using System.Collections;
using UnityEngine;

public class PanelZone : MonoBehaviour
{
    [SerializeField]
    private GameObject explanationPanel;   // 설명패널

    private PlayerController enterPlayer;  // 플레이어
    private Targets targets;               // 과녁들의 Targets 컴포넌트

    private void OnTriggerEnter(Collider other)
    {
        // PlayerController 컴포넌트를 가진 오브젝트에 닿으면
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            enterPlayer = player;
            // zone에 들어가면 패널이 2초 뒤에 사라짐
            StartCoroutine(HideExplanationPanel());
            
            // Targets 오브젝트에서 Targets 컴포넌트를 찾아 과녁 생성 시작
            targets = FindObjectOfType<Targets>(); // 또는 원하는 방식으로 Targets 오브젝트를 찾아야 합니다.
            if (targets != null)
            {
                targets.StartSpawn();
            }
            else
            {
                Debug.LogError("Targets 오브젝트 또는 Targets 컴포넌트를 찾을 수 없습니다.");
            }
        }
        
    }

    private IEnumerator HideExplanationPanel()
    {
        yield return new WaitForSeconds(1f); // 2초 대기

        if (explanationPanel != null)
        {
            explanationPanel.SetActive(false);
            gameObject.SetActive(false);

        }
    }
}
