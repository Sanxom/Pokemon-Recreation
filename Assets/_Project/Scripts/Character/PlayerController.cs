using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;

    [SerializeField] private CharacterAnimator animator;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float moveSpeed;

    private Vector2 inputVector;
    private float overlapRadius = 0.15f;
    private bool isMoving;

    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
    }

    public void HandleUpdate()
    {
        if (isMoving)
            return;

        inputVector = gameInput.GetMovementVectorNormalized();

        if (inputVector != Vector2.zero)
        {
            AnimatePlayer();

            var targetPos = transform.position;
            targetPos.x += inputVector.x;
            targetPos.y += inputVector.y;

            if(IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
        }

        SetMovingStatus(isMoving);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    public Vector2 GetPlayerInputVector()
    {
        return inputVector;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, overlapRadius, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, overlapRadius, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                SetMovingStatus(false);
                OnEncountered?.Invoke();
            }
        }
    }

    private void Interact()
    {
        Vector3 facingDirection = new(animator.MoveX, animator.MoveY);
        Vector3 interactPosition = transform.position + facingDirection;

        // Debug.DrawLine(transform.position, interactPosition, Color.white, 0.5f);

        Collider2D collider = Physics2D.OverlapCircle(interactPosition, overlapRadius, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<IInteractable>()?.Interact();
        }
    }

    public void SetMovingStatus(bool isMoving)
    {
        animator.IsMoving = isMoving;
    }

    private void AnimatePlayer()
    {
        if (GetPlayerInputVector() != Vector2.zero)
        {
            animator.MoveX = GetPlayerInputVector().x;
            animator.MoveY = GetPlayerInputVector().y;
        }
    }
}