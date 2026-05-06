using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public float SmoothSpeed = 0.125f;
    public Vector3 Offset = new Vector3(0f, 0f, -10f);

    private CharacterManager characterManager;
    private Transform target;
    private Transform bigTransform;
    private Transform smallTransform;
    private SmallMovementScript small;

    private void Start()
    {
        characterManager = CharacterManager.Instance;
        bigTransform = GameObject.FindWithTag("Big").transform;
        smallTransform = GameObject.FindWithTag("Small").transform;
        small = smallTransform.GetComponent<SmallMovementScript>();
    }

    private void FixedUpdate()
    {
        target = characterManager.activeCharacter == ActiveCharacter.Big ? bigTransform : smallTransform;
        if (target == null) return;

        Vector3 desiredPosition = target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);

        // rotate camera to match Small's gravity when Small is active
        if (characterManager.activeCharacter == ActiveCharacter.Small)
        {
            float targetCameraAngle = small.CurrentGravity switch
            {
                GravityDirection.Down => 0f,
                GravityDirection.Right => 90f,
                GravityDirection.Up => 180f,
                GravityDirection.Left => 270f,
                _ => 0f
            };
            transform.eulerAngles = new Vector3(0f, 0f,
                Mathf.LerpAngle(transform.eulerAngles.z, targetCameraAngle, SmoothSpeed));
        }
        else
        {
            // reset camera rotation when playing as Big
            transform.eulerAngles = new Vector3(0f, 0f,
                Mathf.LerpAngle(transform.eulerAngles.z, 0f, SmoothSpeed));
        }
    }
}