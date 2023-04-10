using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, IInteractable, ISavable
{
    [SerializeField] private GameObject exclamationGO;
    [SerializeField] private GameObject fovGO;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Dialogue dialogueAfterBattle;
    [SerializeField] private string trainerName;

    private WaitForSeconds battleDelayRoutine;
    private Character character;
    private float battleDelayDuration = 0.5f;
    private bool battleLost = false;

    public string TrainerName => trainerName;
    public Sprite Sprite => sprite;

    private void Awake()
    {
        character = GetComponent<Character>();
        battleDelayRoutine = new WaitForSeconds(battleDelayDuration);
    }

    private void Start()
    {
        SetFOVRotation(character.Animator.DefaultFacingDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
            fovGO.SetActive(false);
        else
            fovGO.SetActive(true);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamationGO.SetActive(true);
        yield return battleDelayRoutine;
        exclamationGO.SetActive(false);

        // Walk towards the Player
        Vector3 difference = player.transform.position - transform.position;
        Vector3 moveVector = difference - difference.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.Move(moveVector);

        // Show Dialogue
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
        {
            GameManager.Instance.StartTrainerBattle(this);
        }));
    }

    public void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
            {
                GameManager.Instance.StartTrainerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogueAfterBattle));
        }
    }

    public void SetFOVRotation(FacingDirection direction)
    {
        float angle = 0f;

        if (direction == FacingDirection.Right)
            angle = 90f;
        else if (direction == FacingDirection.Up)
            angle = 180f;
        else if (direction == FacingDirection.Left)
            angle = 270f;

        fovGO.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void BattleLost()
    {
        battleLost = true;
        fovGO.SetActive(false);
    }
}