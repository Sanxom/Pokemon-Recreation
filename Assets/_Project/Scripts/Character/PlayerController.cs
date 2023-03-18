using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainerView;

    [SerializeField] private GameInput gameInput;

    private Character character;
    private Vector2 inputVector;

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

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, character.OverlapRadius, GameLayers.Instance.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEncountered?.Invoke();
            }
        }
    }

    private void CheckIfInTrainerView()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, character.OverlapRadius, GameLayers.Instance.FOVLayer);

        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainerView?.Invoke(collider);
        }
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
        CheckForEncounters();
        CheckIfInTrainerView();
    }
}