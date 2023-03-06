using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

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

    private void AnimatePlayer()
    {
        if (player.GetPlayerInputVector() != Vector2.zero)
        {
            animator.SetFloat(MOVE_X, player.GetPlayerInputVector().x);
            animator.SetFloat(MOVE_Y, player.GetPlayerInputVector().y);
        }

        animator.SetBool(IS_MOVING, player.IsMoving());
    }
}