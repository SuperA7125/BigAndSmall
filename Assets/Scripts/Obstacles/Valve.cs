using UnityEngine;

public class Valve : MonoBehaviour
{
    [Header("Valve Settings")]
    public RotatingRoom connectedRoom;
    private bool canUse = false;

    private void Update()
    {
        if (CharacterManager.Instance.activeCharacter == ActiveCharacter.Small) return;

        if (canUse && Input.GetKeyDown(KeyCode.E) && !connectedRoom.IsRotating)
        {
            connectedRoom.Rotate();
            CharacterManager.Instance.IsBigBusy = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Big"))
            canUse = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Big"))
            canUse = false;
    }
}