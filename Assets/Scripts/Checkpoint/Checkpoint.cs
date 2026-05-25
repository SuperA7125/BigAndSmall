using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    private Animator animator;
    private Collider2D checkpointCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        checkpointCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Small"))
        {
            activated = true;

            animator.SetBool("IsActivated", true);

            CheckpointManager.Instance.SetCheckpoint(transform.position, this);
        }
    }

    public void PlayRespawn()
    {
        checkpointCollider.enabled = false;

        animator.SetBool("IsRespawning", true);
    }

    // Animation Event
    public void SpawnPlayer()
    {
        CheckpointManager.Instance.SpawnPlayer();
    }

    // Animation Event
    public void OnRespawnAnimationEnd()
    {
        animator.SetBool("IsRespawning", false);
        CharacterManager.Instance.IsSmallDead = false;
        checkpointCollider.enabled = true;
    }
}