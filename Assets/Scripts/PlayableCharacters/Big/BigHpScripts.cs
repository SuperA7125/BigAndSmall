using System.Collections;
using UnityEngine;

public class BigHpScripts : MonoBehaviour
{

    [Header("Big's HP Settings")]

    public int MaxHp = 1000;
    [SerializeField] private int currentHp;
    private Animator animator;

    private void Start()
    {
        currentHp = MaxHp;
        animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if (currentHp >= MaxHp)
        {
            currentHp = MaxHp;
            CharacterManager.Instance.BigNeedsRepair = false;

            // Revive Big if he was dead
            if (CharacterManager.Instance.IsBigDead)
                Revive();
        }
        else
        {
            CharacterManager.Instance.BigNeedsRepair = true;
        }
    }
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die(); // die first, no hitstop on death
            return;
        }

        // only hitstop and hit animation if still alive
        animator.SetTrigger("GotHit");
        StartCoroutine(HitStop());
    }

    public void Heal(int amount)
    {
        currentHp += amount;
    }


    public void Die()
    {
        Debug.Log("Die called!");
        animator.SetBool("IsDead", true);
        CharacterManager.Instance.SetBigDead();
    }

    public void Revive()
    {
        currentHp = MaxHp;
        animator.SetBool("IsDead", false);
        CharacterManager.Instance.IsBigDead = false; // FIX: was never resetting this
    }
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;
    }
}
