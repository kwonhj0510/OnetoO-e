using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    private WeaponRifle  weapon;                     // ���� ������ ��µǴ� ����
    [SerializeField]
    private Status status;

    [Header("Weapon Base")]
    [SerializeField]
    private Image               imageAmmoIcon;              // �Ѿ� ������

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textCurrentAmmo;            // ���� ź�� �� ���
    [SerializeField]
    private TextMeshProUGUI     textMaxAmmo;                // �ִ� ź�� �� ���    

    [Header("HP & Blood Screen")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon is not assigned in the inspector!");
        }
        else
        {
            weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        }

        if (status == null)
        {
            Debug.LogError("Status is not assigned in the inspector!");
        }
        else
        {
            status.onHPEvent.AddListener(UpdateHPHUD);
        }
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textCurrentAmmo.text = $"<size=90>{currentAmmo}</size>";
        textMaxAmmo.text = $"{maxAmmo}";
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = $"{current}";

        if (previous - current > 0 )
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }

    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        float maxAlpha = 47f / 255f;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(maxAlpha, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
