using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Dialogue,
    Cutscene,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera freeRoamCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;

    private TrainerController trainer;
    private GameState currentState;
    private GameState stateBeforePause;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    private void Awake()
    {
        Instance = this;

        ConditionsDatabase.Init();
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;
        DialogueManager.Instance.OnShowDialogue += OnShowDialogue;
        DialogueManager.Instance.OnCloseDialogue += OnCloseDialogue;
    }

    private void OnDestroy()
    {
        battleSystem.OnBattleOver -= EndBattle;
        DialogueManager.Instance.OnShowDialogue -= OnShowDialogue;
        DialogueManager.Instance.OnCloseDialogue -= OnCloseDialogue;
    }

    private void Update()
    {
        if (currentState == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (currentState == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (currentState == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        currentState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        freeRoamCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void StartWildBattle()
    {
        currentState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        freeRoamCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();

        // We create this "Copy" in order to animate this copy instead of animating the base wild Pokemon which is just a "template" Pokemon to spawn
        Pokemon wildPokemonCopy = new(wildPokemon.PokemonBase, wildPokemon.Level);

        battleSystem.StartWildBattle(playerParty, wildPokemonCopy);
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        if (trainer != null)
        {
            currentState = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }

    public void SetCurrentScene(SceneDetails currentScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currentScene;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = currentState;
            currentState = GameState.Paused;
        }
        else
        {
            currentState = stateBeforePause;
        }
    }

    private void EndBattle(bool hasWon)
    {
        if (trainer != null && hasWon)
        {
            trainer.BattleLost();
            trainer = null;
        }

        currentState = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        freeRoamCamera.gameObject.SetActive(true);
    }

    private void OnShowDialogue()
    {
        currentState = GameState.Dialogue;
    }

    private void OnCloseDialogue()
    {
        if (currentState == GameState.Dialogue)
            currentState = GameState.FreeRoam;
    }
}