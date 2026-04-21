using UnityEngine;

public class Lift : MonoBehaviour
{
    private Vector3 startingPos;
    private bool movingToEnd = true;

    [Header("Lift Settings")]
    public Vector3 EndPos;
    public float Speed = 2f;
    public float Tolerance = 0.01f;
    public float WaitTime = 2f;
    public bool IsDoor = false;

    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isActive = true;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        if (!isActive || isWaiting)
        {
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    movingToEnd = !movingToEnd;
                    waitTimer = 0;
                    isWaiting = false;
                }
            }
            return;
        }

        MoveLift();
    }

    void MoveLift()
    {
        Vector3 target = movingToEnd ? EndPos : startingPos;
        transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= Tolerance && !IsDoor)
        {
            waitTimer = WaitTime;
            isWaiting = true;
        }
    }

    public void SetActive() => isActive = true;
    public void SetInactive() => isActive = false;
}