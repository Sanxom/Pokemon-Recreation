using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Dialogue,
    Cutscene
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera freeRoamCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;

    private GameState currentState;

    private void Awake()
    {
        ConditionsDatabase.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        playerController.OnEnterTrainerView += OnEnterTrainerView;
        battleSystem.OnBattleOver += EndBattle;
        DialogueManager.Instance.OnShowDialogue += OnShowDialogue;
        DialogueManager.Instance.OnCloseDialogue += OnCloseDialogue;
    }

    private void OnDestroy()
    {
        playerController.OnEncountered -= StartBattle;
        playerController.OnEnterTrainerView -= OnEnterTrainerView;
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

    private void StartBattle()
    {
        currentState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        freeRoamCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    private void EndBattle(bool hasWon)
    {
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

    private void OnEnterTrainerView(Collider2D trainerCollider)
    {
        TrainerController trainer = trainerCollider.GetComponentInParent<TrainerController>();
        if (trainer != null)
        {
            currentState = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }
}