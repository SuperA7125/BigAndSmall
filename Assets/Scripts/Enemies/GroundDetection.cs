using UnityEngine;
public class GroundDetection : MonoBehaviour
{
    public bool CanContinue = false;
    private int groundCount = 0; // track how many ground objects are touching

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundCount++;
            CanContinue = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundCount--;
            CanContinue = groundCount > 0;
        }
    }
}