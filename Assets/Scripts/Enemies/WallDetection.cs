using UnityEngine;

public class WallDetection : MonoBehaviour
{
    public bool IsAgaisntWall = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            IsAgaisntWall = true;
        }
    }
}
