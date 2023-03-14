using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartyScreen,
    BattleOver
}

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    private WaitForSeconds attackRoutineDelay;
    private WaitForSeconds faintRoutineDelay;
    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    private BattleState state;
    private float attackDelay = 0.5f;
    private float faintDelay = 1f;
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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
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

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerUnit.Pokemon.MoveList);

        yield return dialogueBox.TypeDialogue($"A wild {enemyUnit.Pokemon.PokemonBase.PokemonName} appeared.");

        ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;

        if (sourceUnit.IsPlayerUnit)
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");
        else
            yield return dialogueBox.TypeDialogue($"The enemy {sourceUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");

        sourceUnit.PlayAttackAnimation();
        yield return attackRoutineDelay;
        targetUnit.PlayHitAnimation();

        if (move.Base.Category1 != MoveCategory.Status)
        {
            // Not a Status Move, Apply Damage
            DamageDetails targetDamageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.UnitHUD.UpdateHealth();
            yield return ShowDamageDetails(targetDamageDetails);
        }
        
        if (move.Base.Category1 == MoveCategory.Status || move.Base.Category2 == MoveCategory.Status)
        {
            // Is a Status Move, Apply Effect
            yield return RunMoveEffects(move, sourceUnit, targetUnit);
        }

        if (targetUnit.Pokemon.Health <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{targetUnit.Pokemon.PokemonBase.PokemonName} fainted.");
            targetUnit.PlayFaintAnimation();
            yield return faintRoutineDelay;

            CheckForBattleOver(targetUnit);
        }
    }

    private IEnumerator RunMoveEffects(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        MoveEffects effects = move.Base.Effects;
        Pokemon source = sourceUnit.Pokemon;
        Pokemon target = targetUnit.Pokemon;

        if (effects.BoostList != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                source.ApplyBoosts(effects.BoostList, sourceUnit);
            else
                target.ApplyBoosts(effects.BoostList, targetUnit);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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

    private IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChangeQueue.Count > 0)
        {
            string message = pokemon.StatusChangeQueue.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;

        if (playerUnit.Pokemon.Health > 0)
        {
            currentPokemonFainted = false;
            dialogueBox.EnableActionSelector(false);
            yield return dialogueBox.TypeDialogue($"Come back, {playerUnit.Pokemon.PokemonBase.PokemonName}!");
            playerUnit.PlayFaintAnimation();
            // TODO: Create a new animation for swapping Pokemon.
            yield return faintRoutineDelay;
        }

        yield return SendOutNewPokemon(newPokemon);

        if (currentPokemonFainted)
            ActionSelection();
        else
            StartCoroutine(PerformEnemyMove());
    }

    private IEnumerator SendOutNewPokemon(Pokemon nextPokemon)
    {
        playerUnit.Setup(nextPokemon);
        dialogueBox.SetMoveNames(nextPokemon.MoveList);
        yield return dialogueBox.TypeDialogue($"Go, {nextPokemon.PokemonBase.PokemonName}!");
    }

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.PerformMove;

        Move move = playerUnit.Pokemon.MoveList[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);

        // If the state was not changed by RunMove, then we go to the next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    private IEnumerator PerformPlayerThenEnemyMove()
    {
        state = BattleState.PerformMove;

        Move move = playerUnit.Pokemon.MoveList[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);

        // If the state was not changed by RunMove, then we go to the next step
        if (state == BattleState.PerformMove)
            yield return PerformEnemyMove();
    }

    private IEnumerator PerformEnemyThenPlayerMove()
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        // If the state was not changed by RunMove, then we go to the next step
        if (state == BattleState.PerformMove)
            yield return PerformPlayerMove();
    }

    private IEnumerator PerformEnemyMove()
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        // If the state was not changed by RunMove, then we go to the next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            BattleOver(true);
        }
    }

    private void BattleOver(bool hasWon)
    {
        state = BattleState.BattleOver;

        playerParty.PokemonList.ForEach(p => p.OnBattleOver());

        OnBattleOver?.Invoke(hasWon);
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogueBox.SetDialogue("Choose an action.");
        dialogueBox.EnableActionSelector(true);
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
                    MoveSelection();
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

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
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
            ChooseFirstTurn();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;

        partyScreen.SetPartyData(playerParty.PokemonList);
        partyScreen.gameObject.SetActive(true);
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
            ActionSelection();
        }
    }

    private void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed)
            StartCoroutine(PerformPlayerThenEnemyMove());
        else if (playerUnit.Pokemon.Speed == enemyUnit.Pokemon.Speed)
        {
            int turn = UnityEngine.Random.Range(0, 2);
            if (turn == 0)
                StartCoroutine(PerformPlayerThenEnemyMove());
            else
                StartCoroutine(PerformEnemyThenPlayerMove());
        }
        else
            StartCoroutine(PerformEnemyThenPlayerMove());
    }
}