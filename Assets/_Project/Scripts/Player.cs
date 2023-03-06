using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed;

    private Vector2 inputVector;
    private bool isMoving;

    private void Update()
    {
        inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new(inputVector.x, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        isMoving = moveDir != Vector3.zero;
    }

    public Vector2 GetPlayerInputVector()
    {
        return inputVector;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}