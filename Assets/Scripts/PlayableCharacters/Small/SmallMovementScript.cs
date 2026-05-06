using UnityEngine;
using System.Collections;

public enum GravityDirection { Down, Left, Up, Right }

public class SmallMovementScript : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("Small's Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityStrength = 20f;
    private bool hasJumped = false;
    private bool isRepairingBig = false;

    public LayerMask GroundLayer;
    public Vector2 BoxSize = new Vector2(0.1f, 0.2f);
    public float RayLength;

    [Header("Repair Settings")]
    public float RepairRate = 15f;
    private bool isNearBig = false;
    private BigHpScripts bigHp;
    private float healAccumulator = 0f;

    [Header("Gravity Settings")]
    public GravityDirection CurrentGravity = GravityDirection.Down;
    public bool IsInRoom = false;
    public bool isRoomRotating = false;

    public void SetInRoom(bool value) => IsInRoom = value;
    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // disable default gravity
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (characterManager.activeCharacter == ActiveCharacter.Big) return;
        if (isRoomRotating) return; // block all input while room rotates

        GroundCheck();
        Jump();
        HandleRepair();
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        if (characterManager.activeCharacter == ActiveCharacter.Big ||
            characterManager.IsHacking ||
            isRepairingBig ||
            isRoomRotating) return;

        Move();
    }

    private void ApplyGravity()
    {
        Vector2 gravityVector = GetGravityVector() * gravityStrength;
        rb.AddForce(gravityVector, ForceMode2D.Force);
    }

    private void Move()
    {
        if (horizontalInput == 0) return;

        Vector2 moveDirection = GetMoveDirection() * (horizontalInput * moveSpeed * Time.deltaTime);
        transform.position += (Vector3)moveDirection;
    }

    private void Jump()
    {
        if (characterManager.IsHacking || isRepairingBig) return;
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped)
        {
            hasJumped = true;
            rb.AddForce(-GetGravityVector() * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void GroundCheck()
    {
        Vector2 castDirection = GetGravityVector();
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            BoxSize,
            transform.eulerAngles.z, // match Small's rotation
            castDirection,
            RayLength,
            GroundLayer
        );
        hasJumped = hit.collider == null;
    }

    // Called by RotatingRoom when rotation starts
    public void OnRoomRotationStart()
    {
        isRoomRotating = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // freeze all physics
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
        yield return new WaitForFixedUpdate(); // wait one physics frame
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
        GravityDirection.Left => Vector2.down,  // was Vector2.up
        GravityDirection.Up => Vector2.left,
        GravityDirection.Right => Vector2.up,   // was Vector2.down
        _ => Vector2.right
    };

    private void HandleRepair()
    {
        if (isNearBig && Input.GetMouseButton(1) && characterManager.BigNeedsRepair)
        {
            isRepairingBig = true;
            healAccumulator += RepairRate * Time.deltaTime;
            if (healAccumulator >= 1f)
            {
                bigHp.Heal(Mathf.FloorToInt(healAccumulator));
                healAccumulator = 0f;
                if (CharacterManager.Instance.IsBigDead && !CharacterManager.Instance.BigNeedsRepair)
                    bigHp.Revive();
            }
        }
        else
        {
            isRepairingBig = false;
            healAccumulator = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Big"))
        {
            isNearBig = true;
            bigHp = other.GetComponent<BigHpScripts>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Big"))
        {
            isNearBig = false;
            bigHp = null;
            isRepairingBig = false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 castDirection = GetGravityVector();
        Vector2 castOrigin = (Vector2)transform.position + castDirection * RayLength;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(castOrigin, BoxSize);
    }
}