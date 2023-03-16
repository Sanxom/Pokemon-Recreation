using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private CharacterAnimator animator;
    private float overlapRadius = 0.15f;

    public CharacterAnimator Animator => animator;
    public float OverlapRadius => overlapRadius;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        // Animate
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        // Set Target Position
        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsWalkable(targetPos))
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

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, overlapRadius, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer) != null)
        {
            return false;
        }

        return true;
    }
}