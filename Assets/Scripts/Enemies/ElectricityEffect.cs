using UnityEngine;

public class ElectricityEffect : MonoBehaviour
{
    public float DetectionRange = 1f;
    private GameObject big;
    private SpriteRenderer effect;
    private Animator animator;

    private void Start()
    {
        big = GameObject.FindWithTag("Big");
        effect = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool bigIsNear = Vector2.Distance(transform.position, big.transform.position) <= DetectionRange;
        effect.enabled = !bigIsNear;
        if (animator != null) animator.enabled = !bigIsNear;
    }
}