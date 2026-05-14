using UnityEngine;

public class SmallHpScripts : MonoBehaviour
{
    [Header("Small's HP Settings")]

    public int MaxHp = 1000;
    [SerializeField] private int currentHp;


    private void Start()
    {
        currentHp = MaxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
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
