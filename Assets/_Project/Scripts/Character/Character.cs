using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private CharacterAnimator animator;
    private float overlapRadius = 0.2f;

    public CharacterAnimator Animator => animator;
    public float OverlapRadius => overlapRadius;
    public float OffsetY { get; private set; } = 0.3f;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        // Animate
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        // Set Target Position
        Vector3 targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        // Move
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void LookTowards(Vector3 targetPos)
    {
        float xDifference = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float yDifference = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDifference == 0 || yDifference == 0)
        {
            animator.MoveX = Mathf.Clamp(xDifference, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDifference, -1f, 1f);
        }
        else
        {
            Debug.LogError("Error in LookTowards: You cannot ask a character to look diagonally!");
        }
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        Vector3 difference = targetPos - transform.position;
        Vector3 direction = difference.normalized;
        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, difference.magnitude - 1, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer))
            return false;

        return true;
    }
}