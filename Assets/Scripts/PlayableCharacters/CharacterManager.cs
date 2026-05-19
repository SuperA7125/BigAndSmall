using System;
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
        {
            ChangeCharacter();
        }

        if (IsSmallDead)
        {
            RespawnSmall();
        }
    }

    

    private void ChangeCharacter()
    {
        if (IsBigDead || IsSmallDead) { return; }
        if (activeCharacter == ActiveCharacter.Small)
            {
                activeCharacter = ActiveCharacter.Big;
            }
            else
            {
                activeCharacter = ActiveCharacter.Small;
            }
    }

    private bool isRespawning = false;

    private void RespawnSmall()
    {
        if (isRespawning) return;

        isRespawning = true;

        PerformRespawn();
    }

    private void PerformRespawn()
    {
        CheckpointManager.Instance.RespawnSmall();

        isRespawning = false;
    }
    public void SetSmallDead() 
    {
        IsSmallDead = true;
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
    }
}

public enum ActiveCharacter
{
    Big,
    Small
}
