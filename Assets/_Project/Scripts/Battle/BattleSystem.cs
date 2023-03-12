using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleDialogueBox dialogueBox;

    private WaitForSeconds attackRoutineDelay;
    private WaitForSeconds faintRoutineDelay;
    private BattleState state;
    private float attackDelay = 0.5f;
    private float faintDelay = 2f;
    private int currentAction;
    private int currentMove;

    private void Start()
    {
        attackRoutineDelay = new WaitForSeconds(attackDelay);
        faintRoutineDelay = new WaitForSeconds(faintDelay);
    }

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.SetData(playerUnit.Pokemon);
        enemyHUD.SetData(enemyUnit.Pokemon);

        dialogueBox.SetMoveNames(playerUnit.Pokemon.MoveList);

        yield return dialogueBox.TypeDialogue($"A wild {enemyUnit.Pokemon.PokemonBase.PokemonName} appeared.");

        PlayerAction();
    }

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        Move move = playerUnit.Pokemon.MoveList[currentMove];
        move.PP--;
        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");

        playerUnit.PlayAttackAnimation();
        yield return attackRoutineDelay;
        enemyUnit.PlayHitAnimation();

        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHUD.UpdateHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.PokemonBase.PokemonName} fainted.");
            enemyUnit.PlayFaintAnimation();

            yield return faintRoutineDelay;
            OnBattleOver?.Invoke(true);
        }
        else
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    private IEnumerator PerformEnemyMove()
    {
        state = BattleState.EnemyMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");

        enemyUnit.PlayAttackAnimation();
        yield return attackRoutineDelay;
        playerUnit.PlayHitAnimation();

        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHUD.UpdateHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} fainted.");
            playerUnit.PlayFaintAnimation();
            yield return faintRoutineDelay;
            OnBattleOver?.Invoke(false);
        }
        else
        {
            PlayerAction();
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogueBox.TypeDialogue("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogueBox.TypeDialogue("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogueBox.TypeDialogue("It's not very effective...");

    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogueBox.SetDialogue("Choose an action.");
        //StartCoroutine(dialogueBox.TypeDialogue("Choose an action."));
        dialogueBox.EnableActionSelector(true);
    }

    private void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Run
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Pokemon.MoveList.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Pokemon.MoveList.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogueBox.UpdateMoveSelection(playerUnit.Pokemon.MoveList[currentMove], currentMove);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}