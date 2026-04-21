using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool CanContinue = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            CanContinue = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            CanContinue = true;
        }
        else
        {
            CanContinue = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            CanContinue = false;
        }
    }
}
