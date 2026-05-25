using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public float SmoothSpeed = 0.125f;
    public Vector3 Offset = new Vector3(0f, 0f, -10f);

    private Transform smallTransform;
    private SmallMovementScript small;

    private void Start()
    {
        smallTransform = GameObject.FindWithTag("Small").transform;
        small = smallTransform.GetComponent<SmallMovementScript>();
    }

    private void FixedUpdate()
    {
        if (smallTransform == null) return;

        Vector3 rotatedOffset = smallTransform.rotation * Offset;
        transform.position = Vector3.Lerp(
            transform.position,
            smallTransform.position + rotatedOffset,
            SmoothSpeed
        );

        // follow room live while rotating, otherwise match gravity
        if (small.isRoomRotating && small.CurrentRoom != null)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                small.CurrentRoom.transform.rotation,
                SmoothSpeed
            );
        }
        else
        {
            float targetAngle = small.CurrentGravity switch
            {
                GravityDirection.Down => 0f,
                GravityDirection.Right => 90f,
                GravityDirection.Up => 180f,
                GravityDirection.Left => 270f,
                _ => 0f
            };
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0f, 0f, targetAngle),
                SmoothSpeed
            );
        }
    }
}