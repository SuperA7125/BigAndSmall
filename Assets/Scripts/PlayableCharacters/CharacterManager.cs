using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public ActiveCharacter activeCharacter = ActiveCharacter.Small;

    public bool IsBigDead = false;
    public bool IsSmallDead = false;

    public bool IsHacking = false;
    public bool IsBigBusy = false;
    public bool BigNeedsRepair = false;

    private bool isRespawning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ChangeCharacter();

        if (IsSmallDead && !isRespawning)
        {
            isRespawning = true;
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(1.2f);

        CheckpointManager.Instance.RespawnSmall();
    }

    private void ChangeCharacter()
    {
        if (IsBigDead || IsSmallDead)
            return;

        if (activeCharacter == ActiveCharacter.Small)
            activeCharacter = ActiveCharacter.Big;
        else
            activeCharacter = ActiveCharacter.Small;
    }

    public void SetSmallDead()
    {
        IsSmallDead = true;
    }

    public void FinishRespawn()
    { 
        isRespawning = false;
    }
}

public enum ActiveCharacter
{
    Big,
    Small
}