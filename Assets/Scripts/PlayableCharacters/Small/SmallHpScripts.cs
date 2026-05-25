using UnityEngine;

public class SmallHpScripts : MonoBehaviour
{
    [Header("Small's HP Settings")]
    public int MaxHp = 1000;

    [SerializeField] private int currentHp;

    private bool dead = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SmallMovementScript movement;

    private void Start()
    {
        currentHp = MaxHp;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<SmallMovementScript>();
    }

    public void TakeDamage(int damage)
    {
        if (dead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            dead = true;
            Die();
        }
    }

    private void Die()
    {
        movement.isRespawning = true;

        movement.rb.linearVelocity = Vector2.zero;
        movement.rb.bodyType = RigidbodyType2D.Kinematic;

        animator.SetTrigger("Die");

        CharacterManager.Instance.SetSmallDead();
    }

    // Animation Event at end of death animation
    public void OnDeathAnimationFinished()
    {
        CharacterManager.Instance.SetSmallDead();
        spriteRenderer.enabled = false;
    }

    public void ResetHp()
    {
        dead = false;
        currentHp = MaxHp;
    }
}