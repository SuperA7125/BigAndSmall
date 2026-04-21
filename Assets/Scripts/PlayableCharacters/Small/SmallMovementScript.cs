using UnityEngine;
using UnityEngine.Playables;

public class SmallMovementScript : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("Small's Movement Settings")]

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
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


        if(isNearBig && Input.GetMouseButton(1) && characterManager.BigNeedsRepair)
{
            isRepairingBig = true;
            healAccumulator += RepairRate * Time.deltaTime;
            // In SmallMovementScript
            if (healAccumulator >= 1f)
            {
                bigHp.Heal(Mathf.FloorToInt(healAccumulator));
                healAccumulator = 0f;
                Debug.Log($"IsBigDead: {CharacterManager.Instance.IsBigDead}, BigNeedsRepair: {CharacterManager.Instance.BigNeedsRepair}");

                if (CharacterManager.Instance.IsBigDead && !CharacterManager.Instance.BigNeedsRepair)
                {
                    Debug.Log("Calling Revive!");
                    bigHp.Revive();
                }
            }
        }
        else
        {
            isRepairingBig = false;
            healAccumulator = 0f;
        }
        
    }

    private void FixedUpdate()
    {
        if (characterManager.activeCharacter == ActiveCharacter.Big || characterManager.IsHacking == true || isRepairingBig) {return; }

        Move();
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

    private void Move()
    {
        if (horizontalInput != 0)
        {
            transform.position += Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);
        }
    }


    private void Jump()
    {
        if (characterManager.activeCharacter == ActiveCharacter.Big || characterManager.IsHacking == true || isRepairingBig) { return; }

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
