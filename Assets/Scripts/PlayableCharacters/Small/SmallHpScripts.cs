using UnityEngine;

public class SmallHpScripts : MonoBehaviour
{
    [Header("Small's HP Settings")]

    public int MaxHp = 1000;
    public HpBar hpBar;
    [SerializeField] private int currentHp;


    private void Start()
    {
        currentHp = MaxHp;
        hpBar.Setup(MaxHp, currentHp);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        hpBar.UpdateHp(currentHp);
        if (currentHp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        CharacterManager.Instance.SetSmallDead();
    }
}
