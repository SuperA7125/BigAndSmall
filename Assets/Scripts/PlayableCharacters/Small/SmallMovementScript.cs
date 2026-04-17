using UnityEngine;
using UnityEngine.Playables;

public class SmallMovementScript : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("Small's Movement Settings")]

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private bool hasJumped = false;

    public LayerMask GroundLayer;
    public Vector2 BoxSize = new Vector2(0.1f, 0.2f);
    public float RayLength;


    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        rb = GetComponent<Rigidbody2D>();
    }



    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (characterManager.activeCharacter == ActiveCharacter.Big) { return; }

        GroundCheck();

        Jump();
    }

    private void FixedUpdate()
    {
        if (characterManager.activeCharacter == ActiveCharacter.Big || characterManager.IsHacking == true) {return; }

        Move();
    }
    private void Move()
    {
        if (horizontalInput != 0)
        {
            transform.position += Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);
        }
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped)
        {
            hasJumped = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void GroundCheck()
    {

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxSize, 0f, Vector2.down, RayLength, GroundLayer);


        if (hit.collider != null)
        {
            hasJumped = false;
            return;
        }
        else
        {
            hasJumped = true;
        }
    }
}
