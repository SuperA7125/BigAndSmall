using UnityEngine;
public class WallDetection : MonoBehaviour
{
    public bool IsAgaisntWall = false;
    private int wallCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("PushableObject"))
        {
            wallCount++;
            IsAgaisntWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("PushableObject"))
        {
            wallCount--;
            IsAgaisntWall = wallCount > 0;
        }
    }
}