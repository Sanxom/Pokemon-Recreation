using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private float moveSpeed;

    private Vector2 inputVector;
    private bool isMoving;

    private void Update()
    {
        if (isMoving)
            return;

        inputVector = gameInput.GetMovementVectorNormalized();

        if (inputVector != Vector2.zero)
        {
            var targetPos = transform.position;
            targetPos.x += inputVector.x;
            targetPos.y += inputVector.y;

            if(IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
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
    }

    public Vector2 GetPlayerInputVector()
    {
        return inputVector;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }
}