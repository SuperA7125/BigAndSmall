using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Small Character")]
    public Transform smallCharacter;

    private Vector3 currentCheckpointPosition;
    private Checkpoint activeCheckpoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentCheckpointPosition = smallCharacter.position;
    }

    public void SetCheckpoint(Vector3 newCheckpoint, Checkpoint checkpoint)
    {
        currentCheckpointPosition = newCheckpoint;
        activeCheckpoint = checkpoint;
    }

    public void SpawnPlayer()
    {
        smallCharacter.position = currentCheckpointPosition;

        SpriteRenderer sr = smallCharacter.GetComponent<SpriteRenderer>();
        sr.enabled = true;

        Rigidbody2D rb = smallCharacter.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;

        SmallMovementScript movement = smallCharacter.GetComponent<SmallMovementScript>();
        movement.isRespawning = false;

        CharacterManager.Instance.FinishRespawn();
    }

    public void RespawnSmall()
    {
        if (activeCheckpoint != null)
            activeCheckpoint.PlayRespawn();

        smallCharacter.GetComponent<SmallHpScripts>()?.ResetHp();

        foreach (RotatingRoom room in FindObjectsByType<RotatingRoom>(FindObjectsSortMode.None))
            room.ResetToStart();

        smallCharacter
            .GetComponent<SmallMovementScript>()
            ?.OnRoomRotationEnd(GravityDirection.Down);

        Rigidbody2D rb = smallCharacter.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}