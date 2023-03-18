using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    Up,
    Down, 
    Left, 
    Right
}

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkRightSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;

    // States
    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkRightAnim;
    private SpriteAnimator walkLeftAnim;
    private SpriteAnimator currentAnim;

    // References
    private SpriteRenderer spriteRenderer;

    private bool wasPreviouslyMoving;

    // Properties
    public FacingDirection DefaultFacingDirection => defaultDirection;
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var previousAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        if (currentAnim != previousAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Right)
            MoveX = 1;
        else if (direction == FacingDirection.Left)
            MoveX = -1;
        else if (direction == FacingDirection.Up)
            MoveY = 1;
        else if (direction == FacingDirection.Down)
            MoveY = -1;
    }
}