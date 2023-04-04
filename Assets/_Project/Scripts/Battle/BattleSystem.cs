using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    AboutToUse,
    MoveToForget,
    BattleOver
}

public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
}

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [SerializeField] private GameObject pokeballGO;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private MoveSelectionUI moveSelectionUI;

    private WaitForSeconds oneSecondRoutineDelay;
    private WaitForSeconds twoSecondRoutineDelay;
    private WaitForSeconds attackRoutineDelay;
    private PlayerController player;
    private TrainerController trainer;
    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;
    private MoveBase moveToLearn;
    private BattleState currentState;
    private BattleState? previousState;
    private float attackDelay = 0.5f;
    private float oneSecondDelay = 1f;
    private float twoSecondDelay = 2f;
    private int currentAction;
    private int currentMove;
    private int currentMember;
    private int escapeAttempts;
    private bool isTrainerBattle = false;
    private bool aboutToUseChoice = true;

    private void Start()
    {
        oneSecondRoutineDelay = new WaitForSeconds(oneSecondDelay);
        twoSecondRoutineDelay = new WaitForSeconds(twoSecondDelay);
        attackRoutineDelay = new WaitForSeconds(attackDelay);
    }

    public void StartWildBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        isTrainerBattle = false;

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (currentState == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (currentState == BattleState.MoveSelection)
        {
            if (currentMove > playerUnit.Pokemon.MoveList.Count - 1)
                currentMove = 0;

            HandleMoveSelection();
        }
        else if (currentState == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (currentState == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (currentState == BattleState.MoveToForget)
        {
            Action<int> OnMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    // Don't learn new move
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} did not learn {moveToLearn.MoveName}."));
                }
                else
                {
                    // Forget selected move and learn new move
                    MoveBase selectedMove = playerUnit.Pokemon.MoveList[moveIndex].Base;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}."));
                    playerUnit.Pokemon.MoveList[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                currentState = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(OnMoveSelected);
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            // Wild Pokemon battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);

            dialogueBox.SetMoveNames(playerUnit.Pokemon.MoveList);
            yield return dialogueBox.TypeDialogue($"A wild {enemyUnit.Pokemon.PokemonBase.PokemonName} appeared.");
        }
        else
        {
            // Trainer battle

            // Hide Units and show the Player and Trainer sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} wants to battle!");

            // Send out first Pokemon of the Trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            Pokemon enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} sends out {enemyPokemon.PokemonBase.PokemonName}.");

            // Send out first Pokemon of the Player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            Pokemon playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogueBox.TypeDialogue($"Go, {playerPokemon.PokemonBase.PokemonName}!");
            dialogueBox.SetMoveNames(playerUnit.Pokemon.MoveList);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, BattleUnit sourceUnit, BattleUnit targetUnit, MoveTarget moveTarget)
    {
        Pokemon source = sourceUnit.Pokemon;
        Pokemon target = targetUnit.Pokemon;

        // Stat Changing
        if (effects.BoostList != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.BoostList, sourceUnit);
            else
                target.ApplyBoosts(effects.BoostList, targetUnit);
        }

        // Status Condition
        if (effects.Status != ConditionID.None)
        {
            target.SetStatus(effects.Status);
        }

        // Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.None)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.UnitHUD.UpdateHealth();

            // TODO: This might need testing. Works for now
            sourceUnit.Pokemon.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.UnitHUD.UpdateHealth();
            if (sourceUnit.Pokemon.Health <= 0)
            {
                yield return HandlePokemonFainted(sourceUnit);
            }
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;

        if (sourceUnit.IsPlayerUnit)
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");
        else
            yield return dialogueBox.TypeDialogue($"The enemy {sourceUnit.Pokemon.PokemonBase.PokemonName} used {move.Base.MoveName}.");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return attackRoutineDelay;
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                // Is a Status Move, Apply Effect
                yield return RunMoveEffects(move.Base.Effects, sourceUnit, targetUnit, move.Base.Target);
            }
            else
            {
                // Not a Status Move, Apply Damage
                DamageDetails targetDamageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.UnitHUD.UpdateHealth();
                yield return ShowDamageDetails(targetDamageDetails);
            }

            if (move.Base.SecondaryEffectsList != null && move.Base.SecondaryEffectsList.Count > 0 && targetUnit.Pokemon.Health > 0)
            {
                foreach (SecondaryEffects secondaryEffects in move.Base.SecondaryEffectsList)
                {
                    int rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondaryEffects.Chance)
                        yield return RunMoveEffects(secondaryEffects, sourceUnit, targetUnit, secondaryEffects.Target);
                }
            }

            if (targetUnit.Pokemon.Health <= 0)
            {
                // Target Fainted
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Pokemon.PokemonBase.PokemonName}'s attack missed!");
        }
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        currentState = BattleState.Busy;

        yield return dialogueBox.TypeDialogue($"Choose a move you want to forget over {newMove.MoveName}.");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.MoveList.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        currentState = BattleState.MoveToForget;
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
        playerUnit.Pokemon.CureVolatileStatus();

        if (playerUnit.Pokemon.Health > 0)
        {
            dialogueBox.EnableActionSelector(false);
            yield return dialogueBox.TypeDialogue($"Come back, {playerUnit.Pokemon.PokemonBase.PokemonName}!");
            playerUnit.PlayFaintAnimation();
            // TODO: Create a new animation for swapping Pokemon.
            yield return oneSecondRoutineDelay;
        }

        yield return SendOutNewPokemon(newPokemon);

        if (previousState == null)
            currentState = BattleState.RunningTurn;
        else if (previousState == BattleState.AboutToUse)
        {
            previousState = null;
            StartCoroutine(SendOutNextTrainerPokemon());
        }
    }

    private IEnumerator SendOutNewPokemon(Pokemon nextPokemon)
    {
        playerUnit.Setup(nextPokemon);
        dialogueBox.SetMoveNames(nextPokemon.MoveList);
        yield return dialogueBox.TypeDialogue($"Go, {nextPokemon.PokemonBase.PokemonName}!");
    }

    private IEnumerator SendOutNextTrainerPokemon()
    {
        currentState = BattleState.Busy;

        Pokemon nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} sends out {nextPokemon.PokemonBase.PokemonName}!");

        currentState = BattleState.RunningTurn;
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (currentState == BattleState.BattleOver)
            yield break;
        yield return new WaitUntil(() => currentState == BattleState.RunningTurn);

        // Status like burn or poison will hurt the Pokemon after their turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.UnitHUD.UpdateHealth();
        if (sourceUnit.Pokemon.Health <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => currentState == BattleState.RunningTurn);
        }
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        currentState = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.MoveList[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMoveWithPP();

            // Check who goes first
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed;

                if (playerUnit.Pokemon.Speed == enemyUnit.Pokemon.Speed)
                {
                    // 50/50 chance for either to go first
                    int turn = UnityEngine.Random.Range(0, 2);
                    if (turn == 0)
                        playerGoesFirst = true;
                    else
                        playerGoesFirst = false;
                }
            }

            BattleUnit firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            BattleUnit secondUnit = playerGoesFirst ? enemyUnit : playerUnit;
            Pokemon secondPokemon = secondUnit.Pokemon;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (currentState == BattleState.BattleOver)
                yield break;

            if (secondPokemon.Health > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (currentState == BattleState.BattleOver)
                    yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                Pokemon selectedPokemon = playerParty.PokemonList[currentMember];
                currentState = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogueBox.EnableActionSelector(false);
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogueBox.EnableActionSelector(false);
                yield return TryToEscape();
            }

            // Enemy Turn
            Move enemyMove = enemyUnit.Pokemon.GetRandomMoveWithPP();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (currentState == BattleState.BattleOver)
                yield break;
        }

        if (currentState != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator AboutToUse(Pokemon newPokemon)
    {
        currentState = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} is about to send out {newPokemon.PokemonBase.PokemonName}. Do you want to change Pokemon?");

        currentState = BattleState.AboutToUse;
        dialogueBox.EnableChoiceBox(true);
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogueBox.TypeDialogue($"{faintedUnit.Pokemon.PokemonBase.PokemonName} fainted.");
        faintedUnit.PlayFaintAnimation();
        yield return oneSecondRoutineDelay;

        if (!faintedUnit.IsPlayerUnit)
        {
            // Enemy unit, so gain XP
            int xpYield = faintedUnit.Pokemon.PokemonBase.XpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float divisionNum = 7f;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;
            int xpGain = Mathf.FloorToInt((xpYield * enemyLevel * trainerBonus) / divisionNum);
            playerUnit.Pokemon.XP += xpGain;
            yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} gained {xpGain} xp.");
            yield return playerUnit.UnitHUD.SetXPBarValueSmooth();

            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                // Check level up
                playerUnit.UnitHUD.SetLevel();
                yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} grew to level {playerUnit.Pokemon.Level}!");

                // Try to learn a new Move
                LearnableMove newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.MoveList.Count < PokemonBase.MaxNumOfMoves)
                    {
                        // Learn Move
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} learned {newMove.MoveBase.MoveName}!");
                        dialogueBox.SetMoveNames(playerUnit.Pokemon.MoveList);
                    }
                    else
                    {
                        // Player has 4 Moves, must forget one first
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.PokemonBase.PokemonName} is trying to learn {newMove.MoveBase.MoveName}.");
                        yield return dialogueBox.TypeDialogue($"But {playerUnit.Pokemon.PokemonBase.PokemonName} can't learn more than {PokemonBase.MaxNumOfMoves} moves.");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.MoveBase);
                        yield return new WaitUntil(() => currentState != BattleState.MoveToForget);
                        yield return twoSecondRoutineDelay;
                    }
                }

                yield return playerUnit.UnitHUD.SetXPBarValueSmooth(true);
            }

            yield return oneSecondRoutineDelay;
        }

        CheckForBattleOver(faintedUnit);
    }

    private IEnumerator ThrowPokeball()
    {
        currentState = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue("You can't steal a trainer's Pokemon!");
            currentState = BattleState.RunningTurn;
            yield break;
        }

        Vector3 pokeballSpawnOffset = new(3, -1);
        Vector3 pokeballThrowOffset = new(0, 1);
        Vector3 rotationStrength = new(0, 0, 10f);
        int numOfJumps = 1;
        float fallDuration = 0.5f;
        float throwPower = 2f;
        float throwDuration = 1f;
        float pokeballFallOffset = 1.3f;
        float rotationDuration = 0.8f;
        float pokeballDisappearDuration = 1.5f;
        float pokeballBreakDuration = 0.2f;

        yield return dialogueBox.TypeDialogue($"{player.PlayerName} threw a Pokeball!");

        GameObject pokeballObject = Instantiate(pokeballGO, playerUnit.transform.position - pokeballSpawnOffset, Quaternion.identity);
        SpriteRenderer pokeballSprite = pokeballObject.GetComponent<SpriteRenderer>();

        // Animations
        yield return pokeballSprite.transform.DOJump(enemyUnit.transform.position + pokeballThrowOffset, throwPower, numOfJumps, throwDuration).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeballSprite.transform.DOMoveY(enemyUnit.transform.position.y - pokeballFallOffset, fallDuration).WaitForCompletion();

        // Shaking
        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);
        
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSprite.transform.DOPunchRotation(rotationStrength, rotationDuration).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is caught
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.PokemonBase.PokemonName} was caught!");
            yield return pokeballSprite.DOFade(0f, pokeballDisappearDuration).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.PokemonBase.PokemonName} has been added to your party!");

            Destroy(pokeballObject);
            BattleOver(true);
        }
        else
        {
            // Pokemon broke free
            yield return new WaitForSeconds(1f);
            pokeballSprite.DOFade(0, pokeballBreakDuration);
            yield return enemyUnit.PlayBreakFreeAnimation();

            if (shakeCount < 2)
                yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.PokemonBase.PokemonName} broke free!");
            else
                yield return dialogueBox.TypeDialogue("Almost caught it!");

            Destroy(pokeballObject);
            currentState = BattleState.RunningTurn;
        }
    }

    private IEnumerator TryToEscape()
    {
        currentState = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue("You can't run from trainer battles!");
            currentState = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;
        int playerSpeedMultiplier = 128;
        int enemySpeedAddition = 30;
        int escapeChanceModulus = 256;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogueBox.TypeDialogue("Got away safely!");
            BattleOver(true);
        }
        else
        {
            float chanceToEscape = (playerSpeed * playerSpeedMultiplier) / enemySpeed + enemySpeedAddition * escapeAttempts;
            chanceToEscape %= escapeChanceModulus;

            if (UnityEngine.Random.Range(0, 256) < chanceToEscape)
            {
                yield return dialogueBox.TypeDialogue("Got away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialogue("Can't escape!");
                currentState = BattleState.RunningTurn;
            }
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        // "Magic" numbers are part of the Gen III/IV catching formula
        float a = (3 * pokemon.MaxHealth - 2 * pokemon.Health) * pokemon.PokemonBase.CatchRate * ConditionsDatabase.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHealth);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while(shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65536) >= b)
                break;

            ++shakeCount;
        }

        return shakeCount;
    }

    private bool CheckIfMoveHits(Move move, Pokemon sourcePokemon, Pokemon targetPokemon)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;
        int accuracy = sourcePokemon.StatBoostDictionary[Stat.Accuracy];
        int evasion = targetPokemon.StatBoostDictionary[Stat.Evasion];

        float[] boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        // Accuracy
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        // Evasion
        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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
            if (!isTrainerBattle)
                BattleOver(true);
            else
            {
                Pokemon nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));
                else
                    BattleOver(true);

            }
        }
    }

    private void BattleOver(bool hasWon)
    {
        currentState = BattleState.BattleOver;

        playerParty.PokemonList.ForEach(p => p.OnBattleOver());

        OnBattleOver?.Invoke(hasWon);
    }

    private void ActionSelection()
    {
        currentState = BattleState.ActionSelection;
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
                    StartCoroutine(RunTurns(BattleAction.UseItem));
                    break;
                case 2:
                    // Pokemon
                    previousState = currentState;
                    OpenPartyScreen();
                    break;
                case 3:
                    // Run
                    StartCoroutine(RunTurns(BattleAction.Run));
                    break;
            }
        }
    }

    private void MoveSelection()
    {
        currentState = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.SetDialogue("");
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
            Move move = playerUnit.Pokemon.MoveList[currentMove];
            if (move.PP <= 0)
                return;

            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
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
        currentState = BattleState.PartyScreen;

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

            if (previousState == BattleState.ActionSelection)
            {
                // Player chooses to switch Pokemon during their turn
                previousState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                // Player's Pokemon fainted, forced to switch Pokemon
                currentState = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playerUnit.Pokemon.Health <= 0)
            {
                partyScreen.SetMessageText("You have to choose a Pokemon to continue.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (previousState == BattleState.AboutToUse)
            {
                previousState = null;
                StartCoroutine(SendOutNextTrainerPokemon());
            }
            else
                ActionSelection();
        }
    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogueBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogueBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                // Yes option
                previousState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                // No option
                StartCoroutine(SendOutNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueBox.EnableChoiceBox(false);
            StartCoroutine(SendOutNextTrainerPokemon());
        }
    }
}