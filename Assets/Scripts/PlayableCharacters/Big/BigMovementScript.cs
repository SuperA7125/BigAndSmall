using UnityEngine;

public class BigMovementScript : MonoBehaviour
{
    [Header("Big's Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rayLength = 0.5f;
    [SerializeField] private LayerMask pushMask;

    private CharacterManager characterManager;
    private Animator animator;
    private float horizontalInput;
    private bool facingRight = true;

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
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
            Vector3 direction = facingRight ? Vector3.right : Vector3.left;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, pushMask);
            animator.SetBool("IsPushing", hit.collider != null);
            animator.SetBool("IsWalking", true);

            transform.position += Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);

            if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
                Flip();
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsPushing", false);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}