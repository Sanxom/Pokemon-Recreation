using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private const string MOVE_X = "moveX";
    private const string MOVE_Y = "moveY";
    private const string IS_MOVING = "isMoving";

    private Animator animator;
    private Vector2 input;
    private bool isMoving;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove Diagonal Movement
            if (input.x != 0) 
                input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat(MOVE_X, input.x);
                animator.SetFloat (MOVE_Y, input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool(IS_MOVING, isMoving);
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
}