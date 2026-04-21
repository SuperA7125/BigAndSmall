using System.Collections;
using UnityEngine;

public class EnemyHpScript : MonoBehaviour
{
    [Header("Enemy's HP Settings")]
    public int MaxHp = 250;
    [SerializeField] private int currentHp;
    [SerializeField] private Animator animator;
    private EnemyBehavior enemyBehavior;


    private void Start()
    {
        currentHp = MaxHp;
        animator = GetComponentInChildren<Animator>();
        enemyBehavior = GetComponent<EnemyBehavior>();
    }

    public void TakeDamage(int damage)
    {
        if (enemyBehavior.currentState == EnemyState.Dead) return;
        Debug.Log($"TakeDamage called, animator: {animator}, currentHp: {currentHp}");
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("GotHit");
        enemyBehavior.currentState = EnemyState.Hit;
        StartCoroutine(HitStop());
    }

    public void Die()
    {
        enemyBehavior.currentState = EnemyState.Dead;
        animator.SetBool("IsDead", true);
        enemyBehavior.rb.linearVelocity = Vector2.zero;
        enemyBehavior.rb.bodyType = RigidbodyType2D.Kinematic; // stops all physics
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // wait for death animation to finish then destroy
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;
    }
}