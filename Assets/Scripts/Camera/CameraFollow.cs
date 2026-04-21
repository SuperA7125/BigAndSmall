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

    private void Start()
    {
        characterManager = CharacterManager.Instance;
        bigTransform = GameObject.FindWithTag("Big").transform;
        smallTransform = GameObject.FindWithTag("Small").transform;
    }

    private void FixedUpdate()
    {
        target = characterManager.activeCharacter == ActiveCharacter.Big ? bigTransform : smallTransform;

        if (target == null) return;

        Vector3 desiredPosition = target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
    }
}