using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{
    Idle,
    Walking,
    Dialogue
}

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Vector2> movementPattern;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float timeBetweenPattern;

    private Character character;
    private NPCState currentState;
    private float idleTimer;
    private int currentPatternIndex;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (currentState == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        if (currentState == NPCState.Idle)
        {
            currentState = NPCState.Dialogue;
            character.LookTowards(initiator.position);

            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
            {
                idleTimer = 0f;
                currentState = NPCState.Idle;
            }));
        }
    }

    private IEnumerator Walk()
    {
        Vector3 oldPosition = transform.position;

        currentState = NPCState.Walking;

        yield return character.Move(movementPattern[currentPatternIndex]);

        if (transform.position != oldPosition)
            currentPatternIndex = (currentPatternIndex + 1) % movementPattern.Count;

        currentState = NPCState.Idle;
    }
}