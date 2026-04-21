using System.Collections;
using UnityEngine;

public class BigHpScripts : MonoBehaviour
{

    [Header("Big's HP Settings")]

    public int MaxHp = 1000;
    public HpBar hpBar;
    [SerializeField] private int currentHp;
    private Animator animator;

    private void Start()
    {
        currentHp = MaxHp;
        hpBar.Setup(MaxHp, currentHp);
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
        if (currentHp <= 0) return;

        currentHp -= damage;
        hpBar.UpdateHp(currentHp);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        animator.ResetTrigger("Attack"); // reset any pending attack
        animator.SetTrigger("GotHit");
        StartCoroutine(HitStop());
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        hpBar.UpdateHp(currentHp);
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
        hpBar.UpdateHp(currentHp);
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
