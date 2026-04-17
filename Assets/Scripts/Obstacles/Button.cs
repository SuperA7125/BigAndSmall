using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    [Header("Button Settings")]
    public List<GameObject> Lifts = new List<GameObject>();

    public List<Lift> ConnectedDoors = new List<Lift>();

    [SerializeField] private float hackDuration = 5f;

    [SerializeField] private bool canHack = false;
    [SerializeField] private bool isHacking = false;
    [SerializeField] private bool isHacked = false;
    [SerializeField] private float hackTimer = 0f;

    void Awake()
    {
        SetDoorsInactive();

        DeactivateLifts();
    }

    private void Update()
    {
        if (CharacterManager.Instance.activeCharacter == ActiveCharacter.Big)
        {
            canHack = false;
        }
        if (canHack && !isHacking)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartHacking();
            }
        }

        if (isHacking && hackTimer > 0f)
        {
            hackTimer -= Time.deltaTime;
            if (hackTimer <= 0)
            {
                isHacked = true;
                isHacking = false;
                CharacterManager.Instance.IsHacking = false;
                hackTimer = 0f;
                ActivateLifts();
                SetDoorsActive();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Small") && !isHacked && CharacterManager.Instance.activeCharacter == ActiveCharacter.Small)
        {
            canHack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Small"))
        {
            canHack = false;
        }
    }



    void StartHacking()
    {
        isHacking = true;
        CharacterManager.Instance.IsHacking = true;
        hackTimer = hackDuration;
    }



    void SetDoorsInactive()
    {
        foreach (Lift lift in ConnectedDoors)
        {
            lift.IsDoor = true;
            lift.SetInactive();
        }
    }

    void DeactivateLifts()
    {
        foreach (GameObject lift in Lifts)
        {
            lift.SetActive(false);
        }
    }

    void ActivateLifts()
    {
        foreach (GameObject lift in Lifts)
        {
            lift.SetActive(true);
        }
    }

    void SetDoorsActive()
    {
        foreach (Lift lift in ConnectedDoors)
        {
            lift.SetActive();
        }
    }
}
