using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float MoveSpeed = 2f;
    public float GroundCheckDistance = 0.6f;
    public int Damage = 20;
    public LayerMask playerLayer;

    [SerializeField] private Animator animator;
    [SerializeField] private bool facingRight = true;

    private bool isGrounded = false;
    private bool hasFlippedOnWall = false;

    public Rigidbody2D rb;
    private WallDetection wallDetection;
    private GroundDetection groundDetection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponentInChildren<GroundDetection>();
        wallDetection = GetComponentInChildren<WallDetection>();
    }

    private void Update()
    {
        GroundCheck();
        Patrol();
        UpdateAnimations();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Small"))
            collision.collider.GetComponent<SmallHpScripts>()?.TakeDamage(Damage);
    }

    private void Patrol()
    {
        if (!groundDetection.CanContinue && isGrounded)
        {
            rb.linearVelocity = Vector2.zero;
            Flip(facingRight ? -1f : 1f);
            return;
        }

        if (wallDetection.IsAgaisntWall)
        {
            if (!hasFlippedOnWall)
            {
                Flip(facingRight ? -1f : 1f);
                hasFlippedOnWall = true;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        hasFlippedOnWall = false;
        float intendedDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(intendedDirection * MoveSpeed, rb.linearVelocity.y);
    }

    private void Flip(float direction)
    {
        if (!isGrounded) return;
        if ((direction > 0 && facingRight) || (direction < 0 && !facingRight)) return;

        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f && groundDetection.CanContinue);
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.Linecast(
            transform.position,
            transform.position + Vector3.down * GroundCheckDistance,
            LayerMask.GetMask("Ground")
        ).collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * GroundCheckDistance);
    }
}