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
    Busy,
    PartyScreen
}

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    private WaitForSeconds attackRoutineDelay;
    private WaitForSeconds faintRoutineDelay;
    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    private BattleState state;
    private float attackDelay = 0.5f;
    private float faintDelay = 2f;
    private int currentAction;
    private int currentMove;
    private int currentMember;

    private void Start()
    {
        attackRoutineDelay = new WaitForSeconds(attackDelay);
        faintRoutineDelay = new WaitForSeconds(faintDelay);
    }

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
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
            if (currentMove > playerUnit.Pokemon.MoveList.Count - 1)
                currentMove = 0;

            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        playerHUD.SetData(playerUnit.Pokemon);
        enemyHUD.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

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

            Pokemon nextPokemon = playerParty.GetHealthyPokemon();

            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
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

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.Health > 0)
        {
            dialogueBox.EnableActionSelector(false);
            yield return dialogueBox.TypeDialogue($"Come back, {playerUnit.Pokemon.PokemonBase.PokemonName}!");
            playerUnit.PlayFaintAnimation();
            // TODO: Create a new animation for swapping Pokemon.
            yield return faintRoutineDelay;
        }

        yield return SendOutNewPokemon(newPokemon);

        StartCoroutine(PerformEnemyMove());
    }

    private IEnumerator SendOutNewPokemon(Pokemon nextPokemon)
    {
        playerUnit.Setup(nextPokemon);
        playerHUD.SetData(nextPokemon);
        dialogueBox.SetMoveNames(nextPokemon.MoveList);
        yield return dialogueBox.TypeDialogue($"Go, {nextPokemon.PokemonBase.PokemonName}!");
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogueBox.SetDialogue("Choose an action.");
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
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (currentAction)
            {
                default:
                    break;
                case 0:
                    // Fight
                    PlayerMove();
                    break;
                case 1:
                    // Bag
                    break;
                case 2:
                    // Pokemon
                    OpenPartyScreen();
                    break;
                case 3:
                    // Run
                    break;
            }
        }
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;

        partyScreen.SetPartyData(playerParty.PokemonList);
        partyScreen.gameObject.SetActive(true);
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.MoveList.Count - 1);

        dialogueBox.UpdateMoveSelection(playerUnit.Pokemon.MoveList[currentMove], currentMove);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            PlayerAction();
        }
    }

    private void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.PokemonList.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pokemon selectedMember = playerParty.PokemonList[currentMember];
            if (selectedMember.Health <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Pokemon!");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the same Pokemon!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }
}