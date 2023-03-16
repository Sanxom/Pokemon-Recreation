using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{
    Idle,
    Walking
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
        if (DialogueManager.Instance.IsShowing)
            return;

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

    public void Interact()
    {
        if (currentState == NPCState.Idle)
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }

    private IEnumerator Walk()
    {
        currentState = NPCState.Walking;

        yield return character.Move(movementPattern[currentPatternIndex]);
        currentPatternIndex = (currentPatternIndex + 1) % movementPattern.Count;

        currentState = NPCState.Idle;
    }
}