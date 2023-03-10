using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.InGame.Enable();
    }

    private void OnDestroy()
    {
        playerControls.InGame.Disable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerControls.InGame.Move.ReadValue<Vector2>();

        inputVector = RemoveDiagonalMovement(inputVector);

        inputVector = inputVector.normalized;

        return inputVector;
    }

    private Vector2 RemoveDiagonalMovement(Vector2 inputVector)
    {
        if (inputVector.x != 0)
            inputVector.y = 0;

        return inputVector;
    }
}