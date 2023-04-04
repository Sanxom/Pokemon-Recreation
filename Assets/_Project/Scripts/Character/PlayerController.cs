using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string playerName;

    private Character character;
    private Vector3 transformOffset = new(0, 0.3f);
    private Vector2 inputVector;

    public string PlayerName => playerName;
    public Sprite Sprite => sprite;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (character.IsMoving)
            return;

        inputVector = gameInput.GetMovementVectorNormalized();

        if (inputVector != Vector2.zero)
        {
            StartCoroutine(character.Move(inputVector, OnMoveOver));
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public Vector2 GetPlayerInputVector()
    {
        return inputVector;
    }

    private void Interact()
    {
        Vector3 facingDirection = new(character.Animator.MoveX, character.Animator.MoveY);
        Vector3 interactPosition = transform.position + facingDirection;

        // Debug.DrawLine(transform.position, interactPosition, Color.white, 0.5f);

        Collider2D collider = Physics2D.OverlapCircle(interactPosition, character.OverlapRadius, GameLayers.Instance.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<IInteractable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position - transformOffset, character.OverlapRadius, GameLayers.Instance.TriggerableLayers);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IPlayerTriggerable triggerable))
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
}