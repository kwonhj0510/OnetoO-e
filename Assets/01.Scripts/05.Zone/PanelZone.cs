using System.Collections;
using UnityEngine;

public class PanelZone : MonoBehaviour
{
    [SerializeField]
    private GameObject explanationPanel;   // �����г�

    private PlayerController enterPlayer;  // �÷��̾�
    private Targets targets;               // ������� Targets ������Ʈ

    private void OnTriggerEnter(Collider other)
    {
        // PlayerController ������Ʈ�� ���� ������Ʈ�� ������
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            enterPlayer = player;
            // zone�� ���� �г��� 2�� �ڿ� �����
            StartCoroutine(HideExplanationPanel());
            
            // Targets ������Ʈ���� Targets ������Ʈ�� ã�� ���� ���� ����
            targets = FindObjectOfType<Targets>(); // �Ǵ� ���ϴ� ������� Targets ������Ʈ�� ã�ƾ� �մϴ�.
            if (targets != null)
            {
                targets.StartSpawn();
            }
            else
            {
                Debug.LogError("Targets ������Ʈ �Ǵ� Targets ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        
    }

    private IEnumerator HideExplanationPanel()
    {
        yield return new WaitForSeconds(1f); // 2�� ���

        if (explanationPanel != null)
        {
            explanationPanel.SetActive(false);
            gameObject.SetActive(false);

        }
    }
}
