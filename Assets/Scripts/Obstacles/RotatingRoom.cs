using UnityEngine;

public class RotatingRoom : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float RotationSpeed = 45f;
    public bool IsRotating = false;
    public HpBar rotationProgressBar;

    private float targetAngle;
    private float currentAngle;
    private SmallMovementScript small;

    private void Start()
    {
        currentAngle = transform.eulerAngles.z;
        targetAngle = currentAngle;
        rotationProgressBar.gameObject.SetActive(false);
        small = GameObject.FindWithTag("Small").GetComponent<SmallMovementScript>();
    }

    private void Update()
    {
        if (!IsRotating) return;

        float previousAngle = currentAngle;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, RotationSpeed * Time.deltaTime);
        float angleDelta = currentAngle - previousAngle;
        transform.eulerAngles = new Vector3(0f, 0f, currentAngle);

        if (small != null && small.IsInRoom)
            small.transform.RotateAround(transform.position, Vector3.forward, angleDelta); // remove the minus

        float progress = 1f - Mathf.Abs(targetAngle - currentAngle) / 90f;
        rotationProgressBar.UpdateHp(progress * 100f);

        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            IsRotating = false;
            CharacterManager.Instance.IsBigBusy = false;
            rotationProgressBar.gameObject.SetActive(false);

            if (small != null && small.IsInRoom)
                small.OnRoomRotationEnd(NextGravity(small.CurrentGravity));
            else if (small != null)
                small.isRoomRotating = false; // always unblock even if outside
        }
    }

    public void Rotate()
    {
        targetAngle -= 90f;
        IsRotating = true;

        // always block Small's movement during rotation
        if (small != null)
        {
            small.isRoomRotating = true; // always set regardless of IsInRoom
            if (small.IsInRoom)
                small.OnRoomRotationStart(); // only freeze physics if inside
        }

        rotationProgressBar.Setup(100f, 0f);
        rotationProgressBar.gameObject.SetActive(true);
        CharacterManager.Instance.IsBigBusy = true;
    }

    private GravityDirection NextGravity(GravityDirection current) => current switch
    {
        GravityDirection.Down => GravityDirection.Left,
        GravityDirection.Left => GravityDirection.Up,
        GravityDirection.Up => GravityDirection.Right,
        GravityDirection.Right => GravityDirection.Down,
        _ => GravityDirection.Down
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Small"))
        {
            small.SetInRoom(true);
            small.CurrentRoom = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Small"))
        {
            small.SetInRoom(false);
            small.CurrentRoom = null;
        }
    }
}