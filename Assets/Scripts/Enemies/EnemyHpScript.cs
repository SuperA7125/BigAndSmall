using UnityEngine;

public class EnemyHpScript : MonoBehaviour
{
    [Header("Enemy's HP Settings")]

    public int MaxHp = 250;
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
        Destroy(gameObject);
    }
}
