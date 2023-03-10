using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private const string MOVE_X = "moveX";
    private const string MOVE_Y = "moveY";
    private const string IS_MOVING = "isMoving";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatePlayer();
    }

    public void SetMovingStatus(bool isMoving)
    {
        animator.SetBool(IS_MOVING, isMoving);
    }

    private void AnimatePlayer()
    {
        if (playerController.GetPlayerInputVector() != Vector2.zero)
        {
            animator.SetFloat(MOVE_X, playerController.GetPlayerInputVector().x);
            animator.SetFloat(MOVE_Y, playerController.GetPlayerInputVector().y);
        }

        SetMovingStatus(playerController.IsMoving());
    }
}