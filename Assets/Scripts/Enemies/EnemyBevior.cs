using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float MoveSpeed = 2f;
    public float DetectionRange = 1.5f;
    public float AttackRange = 0.5f;
    public float GroundCheckDistance = 0.6f; // BUG FIX: was hardcoded 0.01f, far too short

    public bool IsSeeingBig = false;
    public bool IsSeeingSmall = false;
    [SerializeField]private Animator animator;

    public GameObject Big;
    public GameObject Small;
    public LayerMask playerLayer;
    public CircleCollider2D detectionCollider;

    public Transform AttackPoint;
    public float AttackRadius = 0.3f;
    public int Damage = 20;

    [SerializeField] private bool facingRight = true;
    private bool isGrounded = false;
    private bool hasFlippedOnWall = false;

    public Rigidbody2D rb;
    private WallDetection wallDetection;
    private GroundDetection groundDetection;
    public EnemyState currentState = EnemyState.Idle;
    private float intendedDirection = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponentInChildren<GroundDetection>();
        wallDetection = GetComponentInChildren<WallDetection>();

        Big = GameObject.FindWithTag("Big");
        Small = GameObject.FindWithTag("Small");

        if (detectionCollider == null)
            Debug.LogError("EnemyBehavior requires a CircleCollider2D for detection.");
        else
        {
            detectionCollider.radius = DetectionRange;
            detectionCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        GroundCheck();

        switch (currentState)
        {
            case EnemyState.Idle: Patrol(); break;
            case EnemyState.Chasing: ChasePlayer(); break;
            case EnemyState.Attacking: AttackPlayer(); break;
            case EnemyState.Dead: return;
            case EnemyState.Hit: return; // freeze everything during hit
        }

        UpdateAnimations();
    }


    private void OnTriggerEnter2D(Collider2D other) => HandleTrigger(other);
    private void OnTriggerStay2D(Collider2D other) => HandleTrigger(other);

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        SetSeeingFlag(other, false);
        if (!IsSeeingBig && !IsSeeingSmall)
            currentState = EnemyState.Idle;
    }

    private void HandleTrigger(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        bool blocked = Physics2D.Linecast(
            transform.position,
            other.transform.position,
            LayerMask.GetMask("Ground")
        ).collider != null;

        if (blocked)
        {
            SetSeeingFlag(other, false);
            if (!IsSeeingBig && !IsSeeingSmall)
                currentState = EnemyState.Idle;
            return;
        }

        SetSeeingFlag(other, true);

        // FIX: don't interrupt an active attack
        if (currentState == EnemyState.Attacking) return;

        if (groundDetection.CanContinue && !wallDetection.IsAgaisntWall)
            currentState = EnemyState.Chasing;
    }

    private void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;

        GameObject target = IsSeeingSmall ? Small : (IsSeeingBig ? Big : null);
        if (target == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        // Always face the target while attacking
        float direction = Mathf.Sign(target.transform.position.x - transform.position.x);
        intendedDirection = direction;
        Flip(direction);  // but this won't work since Flip blocks during Attacking state...

        bool stillInRange = Physics2D.Raycast(
            transform.position,
            target.transform.position - transform.position,
            AttackRange,
            playerLayer
        ).collider != null;

        if (!stillInRange)
            currentState = EnemyState.Chasing;
    }
    private void ChasePlayer()
    {
        if (wallDetection.IsAgaisntWall || !groundDetection.CanContinue)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = EnemyState.Idle;
            return;
        }

        GameObject target = (IsSeeingSmall) ? Small : (IsSeeingBig ? Big : null);
        if (target == null) return;

        bool inAttackRange = Physics2D.Raycast(
            transform.position,
            target.transform.position - transform.position,
            AttackRange,
            playerLayer
        ).collider != null;

        if (inAttackRange)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = EnemyState.Attacking;
            return;
        }

        float direction = Mathf.Sign(target.transform.position.x - transform.position.x);
        intendedDirection = direction; // store intended direction
        Flip(intendedDirection);
        rb.linearVelocity = new Vector2(direction * MoveSpeed, rb.linearVelocity.y);
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
        intendedDirection = facingRight ? 1f : -1f; // store intended direction
        rb.linearVelocity = new Vector2(intendedDirection * MoveSpeed, rb.linearVelocity.y);
    }

    public void OnAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(AttackPoint.transform.position, AttackRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Big"))
            {
                hit.GetComponent<BigHpScripts>()?.TakeDamage(Damage);
            }
            else if (hit.CompareTag("Small"))
            {
                hit.GetComponent<SmallHpScripts>()?.TakeDamage(Damage);
            }
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", currentState == EnemyState.Chasing || currentState == EnemyState.Idle && rb.linearVelocity.x != 0);
        animator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
    }
    private void GroundCheck()
    {
        isGrounded = Physics2D.Linecast(
            transform.position,
            transform.position + Vector3.down * GroundCheckDistance,
            LayerMask.GetMask("Ground")
        ).collider != null;
    }
    private void Flip(float direction)
    {
        if (!isGrounded) return;
        //if (currentState == EnemyState.Attacking) return;
        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(intendedDirection) && rb.linearVelocity.x != 0) return; // FIX: being pushed, ignore
        if ((direction > 0 && facingRight) || (direction < 0 && !facingRight)) return;

        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private bool IsPlayer(Collider2D col) =>
        col.CompareTag("Small") || col.CompareTag("Big");

    private void SetSeeingFlag(Collider2D col, bool value)
    {
        if (col.CompareTag("Small")) IsSeeingSmall = value;
        else if (col.CompareTag("Big")) IsSeeingBig = value;
    }

    public void OnHitAnimationEnd()
    {
        animator.SetBool("IsAttacking", false);
        currentState = EnemyState.Chasing;
    }
    public void OnAttackAnimationEnd()
    {
        animator.SetBool("IsAttacking", false);
        currentState = EnemyState.Chasing;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * GroundCheckDistance);
        Gizmos.DrawWireSphere(AttackPoint.transform.position, AttackRadius);
    }
}

public enum EnemyState { Idle, Chasing, Attacking, Dead, Hit }