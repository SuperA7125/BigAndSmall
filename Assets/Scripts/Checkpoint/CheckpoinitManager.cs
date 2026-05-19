using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Small Character")]
    public Transform smallCharacter;

    private Vector3 currentCheckpointPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Default spawn point
        currentCheckpointPosition = smallCharacter.position;
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpointPosition = newCheckpoint;
    }

    public void RespawnSmall()
    {
        CharacterManager.Instance.IsSmallDead = false;

        smallCharacter.position = currentCheckpointPosition;

        // Optional:
        Rigidbody2D rb = smallCharacter.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}