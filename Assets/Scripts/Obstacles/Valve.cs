using UnityEngine;

public class Valve : MonoBehaviour
{
    [Header("Valve Settings")]
    public RotatingRoom connectedRoom;
    public Transform valveChild; // drag the child object here in Inspector
    private bool canUse = false;

    private void Update()
    {
        if (canUse && Input.GetKeyDown(KeyCode.E) && !connectedRoom.IsRotating)
        {
            connectedRoom.Rotate();
            CharacterManager.Instance.IsBigBusy = true;
            RotateChild();
        }
    }

    private void RotateChild()
    {
        StartCoroutine(RotateOverTime());
    }

    private System.Collections.IEnumerator RotateOverTime()
    {
        float elapsed = 0f;
        float duration = 90f / connectedRoom.RotationSpeed;
        Quaternion startRot = valveChild.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, -90f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            valveChild.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            yield return null;
        }

        valveChild.localRotation = endRot;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Small"))
            canUse = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Small"))
            canUse = false;
    }
}