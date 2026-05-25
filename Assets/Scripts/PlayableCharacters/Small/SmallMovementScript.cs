using UnityEngine;
using System.Collections;

public enum GravityDirection { Down, Left, Up, Right }

public class SmallMovementScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityStrength = 20f;
    public float JumpUpDuration = 0.2f;

    [Header("Ground Check")]
    public LayerMask GroundLayer;
    public Vector2 BoxSize = new Vector2(0.1f, 0.2f);
    public float RayLength;

    [Header("Gravity Settings")]
    public GravityDirection CurrentGravity = GravityDirection.Down;
    public bool IsInRoom = false;
    public bool isRoomRotating = false;

    // Private fields
    private CharacterManager characterManager;
    public Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private float jumpUpTimer = 0f;
    private float coyoteTime = 0.15f;
    private float coyoteTimer = 0f;
    private bool hasJumped = false;
    public RotatingRoom CurrentRoom; // add this back
    public bool isRespawning = false;
    // --- Unity Methods ---

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (isRoomRotating)
        {
            ResetAnimations();
            return;
        }

        if (isRespawning)
        {
            return;
        }

        GroundCheck();
        if (!characterManager.IsHacking)
            Jump();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isRoomRotating || isRespawning) return; // add isRespawning
        ApplyGravity();
        if (characterManager.IsHacking) return;
        Move();
    }




    // --- Movement ---

    private void Move()
    {
        if (horizontalInput == 0) return;
        Vector2 moveDirection = GetMoveDirection() * (horizontalInput * moveSpeed);
        Vector2 gravityVelocity = GetGravityVector() * Vector2.Dot(rb.linearVelocity, GetGravityVector());
        rb.linearVelocity = moveDirection + gravityVelocity;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (!hasJumped || coyoteTimer > 0f))
        {
            hasJumped = true;
            coyoteTimer = 0f;
            jumpUpTimer = JumpUpDuration;
            rb.AddForce(-GetGravityVector() * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void ApplyGravity()
    {
        rb.AddForce(GetGravityVector() * gravityStrength, ForceMode2D.Force);
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, BoxSize,
            transform.eulerAngles.z,
            GetGravityVector(), RayLength, GroundLayer
        );

        if (hit.collider != null)
        {
            float dot = Vector2.Dot(hit.normal, -GetGravityVector());
            if (dot >= 0.5f)
            {
                hasJumped = false;
                coyoteTimer = coyoteTime;
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0)
                hasJumped = true;
        }
    }

    // --- Animations ---

    private void ResetAnimations()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsJumping", false);
        animator.SetFloat("JumpVelocity", 0f);
    
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsDead", characterManager.IsSmallDead);
        if (characterManager.IsHacking)
        {
            ResetAnimations();
            return;
        }

        jumpUpTimer -= Time.deltaTime;
        float jumpVelocity = hasJumped && jumpUpTimer <= 0f
            ? Vector2.Dot(rb.linearVelocity, -GetGravityVector())
            : 1f;

        animator.SetBool("IsWalking", !hasJumped && Mathf.Abs(horizontalInput) > 0.1f);
        animator.SetBool("IsJumping", hasJumped);
        animator.SetFloat("JumpVelocity", jumpVelocity);

        spriteRenderer.flipX = horizontalInput < 0;
    }

    // --- Room Rotation ---

    public void SetInRoom(bool value) => IsInRoom = value;

    public void OnRoomRotationStart()
    {
        isRoomRotating = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OnRoomRotationEnd(GravityDirection newGravity)
    {
        CurrentGravity = newGravity;
        isRoomRotating = false;
        UpdateRotation();
        StartCoroutine(RestorePhysics());
    }

    private IEnumerator RestorePhysics()
    {
        yield return new WaitForFixedUpdate();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void UpdateRotation()
    {
        float angle = CurrentGravity switch
        {
            GravityDirection.Down => 0f,
            GravityDirection.Right => 90f,
            GravityDirection.Up => 180f,
            GravityDirection.Left => 270f,
            _ => 0f
        };
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    // --- Helpers ---

    private Vector2 GetGravityVector() => CurrentGravity switch
    {
        GravityDirection.Down => Vector2.down,
        GravityDirection.Left => Vector2.left,
        GravityDirection.Up => Vector2.up,
        GravityDirection.Right => Vector2.right,
        _ => Vector2.down
    };

    private Vector2 GetMoveDirection() => CurrentGravity switch
    {
        GravityDirection.Down => Vector2.right,
        GravityDirection.Left => Vector2.down,
        GravityDirection.Up => Vector2.left,
        GravityDirection.Right => Vector2.up,
        _ => Vector2.right
    };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            (Vector2)transform.position + GetGravityVector() * RayLength,
            BoxSize
        );
    }
}