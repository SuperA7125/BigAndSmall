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
    private float waitTimer = 0f;
    public bool isWaiting = false;

    public bool IsDoor = false;
    private bool isActive = true;
    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        TogglePlatforms();
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
    }

    void TogglePlatforms()
    {
        if (!isActive) return;
        Vector3 target = movingToEnd ? EndPos : startingPos;


        transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= Tolerance && !IsDoor && !isWaiting)
        {
            waitTimer = WaitTime;
            isWaiting = true;
        }
    }

    public void SetActive()
    {
        isActive = true;
    }

    public void SetInactive()
    {
        isActive = false;
    }
}
