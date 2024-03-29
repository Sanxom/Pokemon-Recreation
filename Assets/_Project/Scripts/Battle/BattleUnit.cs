using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private BattleHUD unitHUD;
    [SerializeField] private bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }
    public BattleHUD UnitHUD => unitHUD;
    public bool IsPlayerUnit => isPlayerUnit;

    private Image image;
    private Color originalColor;
    private Vector3 originalPosition;

    // DOTween vars
    private Vector3 originalAnimationScale = Vector3.one;
    private Vector3 captureAnimationScale = new(0.3f, 0.3f, 1f);
    private float enterAnimationStartX = 500f;
    private float attackMoveAmount = 50f;
    private float enterAnimationDuration = 1f;
    private float attackAnimationDuration = 0.25f;
    private float hitAnimationDuration = 0.1f;
    private float faintAnimationDuration = 0.5f;
    private float faintUnitOffset = 150f;
    private float captureAnimationDuration = 0.5f;
    private float captureUnitOffset = 50f;

    private void Awake()
    {
        image = GetComponent<Image>();

        originalColor = image.color;
        originalPosition = image.transform.localPosition;
    }

    public IEnumerator PlayCaptureAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, captureAnimationDuration));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y + captureUnitOffset, captureAnimationDuration));
        sequence.Join(transform.DOScale(captureAnimationScale, captureAnimationDuration));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakFreeAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, captureAnimationDuration));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y, captureAnimationDuration));
        sequence.Join(transform.DOScale(originalAnimationScale, captureAnimationDuration));
        yield return sequence.WaitForCompletion();
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;

        if (isPlayerUnit)
            image.sprite = Pokemon.PokemonBase.BackSprite;
        else
            image.sprite = Pokemon.PokemonBase.FrontSprite;

        unitHUD.gameObject.SetActive(true);
        unitHUD.SetData(pokemon);

        transform.localScale = Vector3.one;
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void Clear()
    {
        unitHUD.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-enterAnimationStartX, originalPosition.y);
        else
            image.transform.localPosition = new Vector3(enterAnimationStartX, originalPosition.y);

        image.transform.DOLocalMoveX(originalPosition.x, enterAnimationDuration);
    }

    public void PlayAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + attackMoveAmount, attackAnimationDuration));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - attackMoveAmount, attackAnimationDuration));

        sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, attackAnimationDuration));
    }

    public void PlayHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, hitAnimationDuration));
        sequence.Append(image.DOColor(originalColor, hitAnimationDuration));
    }

    public void PlayFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - faintUnitOffset, faintAnimationDuration));
        sequence.Join(image.DOFade(0f, faintAnimationDuration));
    }
}