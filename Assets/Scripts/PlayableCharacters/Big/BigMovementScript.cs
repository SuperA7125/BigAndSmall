using UnityEngine;


public class BigMovementScript : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("Big's Movement Settings")]

    private Animator animator;

    [SerializeField] private int damage = 50;


    [SerializeField] private LayerMask pushMask;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float rayLength = 0.5f;

    public GameObject AttackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayers;


    private float horizontalInput;

    private bool facingRight = true;

    private Vector3 diraction;

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        animator = GetComponent<Animator>();
    }



    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0) && characterManager.activeCharacter == ActiveCharacter.Big && !IsAnimationRunning("StartAttackBool"))
        {
            StartAttackBool();            
        }

        
    }

    private void FixedUpdate()
    {
        if (CharacterManager.Instance.IsBigBusy) return;
        if (characterManager.activeCharacter == ActiveCharacter.Small) 
        {
            animator.SetBool("IsWalking", false); 
            animator.SetBool("IsPushing", false);
            return;
        }

        Move();
    }
    private void Move()
    {
        if (horizontalInput != 0)
        {
            diraction = facingRight ? Vector3.right : Vector3.left;
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, diraction , rayLength ,pushMask);
            if (hit2D.collider != null)
            {
                animator.SetBool("IsPushing", true);
            }
            else
            {
                animator.SetBool("IsPushing", false);
            }
        
        animator.SetBool("IsWalking", true);
            transform.position += Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);

            if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            {
                Flip();
            }
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsPushing", false);
        }   
    }

    private void Attack()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(AttackPoint.transform.position, attackRadius , enemyLayers);


        foreach (Collider2D enemyGameObject in enemies)
        {
            if (enemyGameObject.CompareTag("Enemy"))
            {
                EnemyHpScript enemy = enemyGameObject.GetComponent<EnemyHpScript>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }


    private void StartAttackBool()
    {
        animator.SetBool("IsAttacking", true);
    }
    private void EndAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

    bool IsAnimationRunning(string animationName)
    {
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentStateInfo.IsName(animationName))
        {
            if (currentStateInfo.normalizedTime < 0.95f)
            {
                return true;
            }
        }
        return false;
    }
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(AttackPoint.transform.position, attackRadius);
    }
}
