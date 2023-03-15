using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private string moveX = "moveX";
    private string moveY = "moveY";
    private const string IS_MOVING = "isMoving";

    private Animator animator;

    public Animator Animator => animator;
    public string MoveX => moveX;
    public string MoveY => moveY;

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
            animator.SetFloat(moveX, playerController.GetPlayerInputVector().x);
            animator.SetFloat(moveY, playerController.GetPlayerInputVector().y);
        }

        SetMovingStatus(playerController.IsMoving());
    }
}