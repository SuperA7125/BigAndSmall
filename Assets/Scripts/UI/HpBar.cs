using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image filler;
    private float maxHp;
    private float currentHp;

    public void Setup(float max, float current)
    {
        maxHp = max;
        currentHp = current;
        UpdateBar();
    }

    public void UpdateHp(float current)
    {
        currentHp = current;
        UpdateBar();
    }

    private void UpdateBar()
    {
        filler.fillAmount = Mathf.Clamp01(currentHp / maxHp);
    }
}