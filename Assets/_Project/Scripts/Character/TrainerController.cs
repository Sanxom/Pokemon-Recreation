using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private GameObject exclamationGO;
    [SerializeField] private GameObject fovGO;
    [SerializeField] private Dialogue dialogue;

    private WaitForSeconds battleDelayRoutine;
    private Character character;
    private float battleDelayDuration = 0.5f;

    private void Awake()
    {
        character = GetComponent<Character>();
        battleDelayRoutine = new WaitForSeconds(battleDelayDuration);
    }

    private void Start()
    {
        SetFOVRotation(character.Animator.DefaultFacingDirection);
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
            print("Starting Trainer Battle!");
        }));
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
}