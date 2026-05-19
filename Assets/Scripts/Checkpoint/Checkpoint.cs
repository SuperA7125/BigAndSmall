using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Small"))
        {
            activated = true;

            CheckpointManager.Instance.SetCheckpoint(transform.position);

            Debug.Log("Checkpoint Activated");
        }
    }
}