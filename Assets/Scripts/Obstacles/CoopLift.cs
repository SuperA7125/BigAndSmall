using UnityEngine;

public class CoopLift : MonoBehaviour
{
    [Header("Lift Settings")]
    public Vector3 BottomPos;
    public Vector3 TopPos;
    public float Speed = 2f;
    public float Tolerance = 0.01f;
    public float WaitTimeAtTop = 2f;

    private bool bigOnLift = false;
    private bool smallOnLift = false;
    private bool movingUp = false;
    private bool waitingAtTop = false;
    private float waitTimer = 0f;

    private bool isActive = false;

    [SerializeField] private LayerMask bigLayer;
    [SerializeField] private LayerMask smallLayer;

    [SerializeField] private Rigidbody2D bigRb;
    [SerializeField] private Rigidbody2D smallRb;

    private SpriteRenderer spriteRenderer;
    private Collider2D liftCollider;


    void Start()
    {
        transform.position = TopPos;

        spriteRenderer = GetComponent<SpriteRenderer>();
        liftCollider = GetComponent<Collider2D>();

        // Start hidden and disabled
        liftCollider.enabled = false;
    }

    void Update()
    {
        if (!isActive) return;

        bool bothOn = bigOnLift && smallOnLift;

        // Waiting at top until both leave
        if (waitingAtTop)
        {
            if (!bothOn)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                    waitingAtTop = false; // start going back down
            }
            return;
        }

        // Go up only when both are on
        if (bothOn && !movingUp)
            movingUp = true;

        // Go back down if either leaves mid-ride
        if (!bothOn && movingUp)
            movingUp = false;

        Vector3 target = movingUp ? TopPos : BottomPos;
        Vector3 delta = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime) - transform.position;
        transform.position += delta;

        // Move riders with the lift to fix jitter
        if (bigOnLift && bigRb != null)
            bigRb.position += new Vector2(delta.x, delta.y);
        if (smallOnLift && smallRb != null)
            smallRb.position += new Vector2(delta.x, delta.y);

        // Arrived at top
        if (movingUp && Vector3.Distance(transform.position, TopPos) <= Tolerance)
        {
            waitTimer = WaitTimeAtTop;
            waitingAtTop = true;
            movingUp = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name} layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        if (other.CompareTag("Big"))
        {
            bigOnLift = true;
            bigRb = other.GetComponentInParent<Rigidbody2D>();
            Debug.Log("Big is on the lift");
        }
        else if (other.CompareTag("Small"))
        {
            smallOnLift = true;
            smallRb = other.GetComponentInParent<Rigidbody2D>();
            Debug.Log("Small is on the lift");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Big"))
        {
            bigOnLift = false;
            bigRb = null;
        }
        else if (other.CompareTag("Small"))
        {
            smallOnLift = false;
            smallRb = null;
        }
    }

    public void SetActive()
    {
        isActive = true;
        //spriteRenderer.enabled = true;
        liftCollider.enabled = true;
    }
    public void SetInactive() => isActive = false;
}