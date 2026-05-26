using UnityEngine;

public class RotatingRoom : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float RotationSpeed = 45f;
    public bool IsRotating = false;
    //public HpBar rotationProgressBar;

    [Header("Starting Rotation")]
    public float StartingRotation = 0f;

    private float targetAngle;
    private float currentAngle;
    private SmallMovementScript small;

    private void Start()
    {
        transform.eulerAngles = new Vector3(0f, 0f, StartingRotation);
        currentAngle = StartingRotation;
        targetAngle = StartingRotation;
        //rotationProgressBar.gameObject.SetActive(false);
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
            small.transform.RotateAround(transform.position, Vector3.forward, angleDelta);

        float progress = 1f - Mathf.Abs(targetAngle - currentAngle) / 90f;
        //rotationProgressBar.UpdateHp(progress * 100f);

        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            IsRotating = false;
            //rotationProgressBar.gameObject.SetActive(false);

            if (small != null && small.IsInRoom)
                small.OnRoomRotationEnd(NextGravity(small.CurrentGravity));
            else if (small != null)
                small.isRoomRotating = false;
        }
    }

    public void Rotate()
    {
        targetAngle -= 90f;
        IsRotating = true;

        if (small != null)
        {
            small.isRoomRotating = true;
            if (small.IsInRoom)
                small.OnRoomRotationStart();
        }

        //rotationProgressBar.Setup(100f, 0f);
        //rotationProgressBar.gameObject.SetActive(true);
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
            small.CurrentRoom = this; // add back
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Small"))
        {
            small.SetInRoom(false);
            small.CurrentRoom = null; // add back
        }
    }

    public void ResetToStart()
    {
        targetAngle = StartingRotation;
        IsRotating = true;
        // don't block small or show progress bar, this is a death reset
    }
}