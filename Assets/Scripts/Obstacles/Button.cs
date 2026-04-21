using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    public List<Lift> ConnectedLifts = new List<Lift>();
    public List<Lift> ConnectedDoors = new List<Lift>();
    public List<CoopLift> ConnectedCoopLifts = new List<CoopLift>();

    public HpBar hackProgressBar;
    [SerializeField] private float hackDuration = 5f;
    [SerializeField] private bool canHack = false;
    [SerializeField] private bool isHacking = false;
    [SerializeField] private bool isHacked = false;
    [SerializeField] private float hackTimer = 0f;

    void Awake()
    {
        SetDoorsInactive();
        DeactivateLifts();
        DeactivateCoopLifts(); // add this
        hackProgressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (CharacterManager.Instance.activeCharacter == ActiveCharacter.Big)
            canHack = false;

        if (canHack && !isHacking && Input.GetKeyDown(KeyCode.E))
            StartHacking();

        if (isHacking && hackTimer > 0f)
        {
            hackTimer -= Time.deltaTime;
            if (hackTimer <= 0)
            {
                isHacked = true;
                hackProgressBar.gameObject.SetActive(false);
                isHacking = false;
                CharacterManager.Instance.IsHacking = false;
                hackTimer = 0f;
                ActivateLifts();
                SetDoorsActive();
            }
        }

        if (isHacking)
            hackProgressBar.UpdateHp(hackTimer);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Small") && !isHacked && CharacterManager.Instance.activeCharacter == ActiveCharacter.Small)
            canHack = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Small"))
            canHack = false;
    }

    void StartHacking()
    {
        isHacking = true;
        CharacterManager.Instance.IsHacking = true;
        hackTimer = hackDuration;
        hackProgressBar.Setup(hackDuration, hackDuration); // max = hackDuration, current = full
        hackProgressBar.gameObject.SetActive(true); // show on start
    }

    void SetDoorsInactive()
    {
        foreach (Lift lift in ConnectedDoors)
        {
            lift.IsDoor = true;
            lift.SetInactive();
        }
    }

    void SetDoorsActive()
    {
        foreach (Lift lift in ConnectedDoors)
            lift.SetActive();
    }

    void DeactivateLifts()
    {
        foreach (Lift lift in ConnectedLifts)
            lift.SetInactive(); // was lift.SetActive(false)
    }

    void ActivateLifts()
    {
        foreach (Lift lift in ConnectedLifts)
            lift.SetActive(); // was lift.SetActive(true)
        foreach (CoopLift lift in ConnectedCoopLifts)
            lift.SetActive();
    }

    void DeactivateCoopLifts()
    {
        foreach (CoopLift lift in ConnectedCoopLifts)
            lift.SetInactive();
    }
}