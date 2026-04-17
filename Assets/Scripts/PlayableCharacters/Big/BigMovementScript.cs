using UnityEngine;


public class BigMovementScript : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("Big's Movement Settings")]

    [SerializeField] private float moveSpeed = 5f;

    private float horizontalInput;

    private void Awake()
    {
        characterManager = GetComponentInParent<CharacterManager>();
    }



    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (characterManager.activeCharacter == ActiveCharacter.Small) { return; }

        Move();
    }
    private void Move()
    {
        if (horizontalInput != 0)
        {
            transform.position += Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);
        }
    }
}
