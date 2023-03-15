using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Dialogue
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera freeRoamCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;

    private GameState state;

    private void Awake()
    {
        ConditionsDatabase.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        DialogueManager.Instance.OnShowDialogue += OnShowDialogue;
        DialogueManager.Instance.OnCloseDialogue += OnCloseDialogue;
    }

    private void OnDestroy()
    {
        playerController.OnEncountered -= StartBattle;
        battleSystem.OnBattleOver -= EndBattle;
        DialogueManager.Instance.OnShowDialogue -= OnShowDialogue;
        DialogueManager.Instance.OnCloseDialogue -= OnCloseDialogue;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        freeRoamCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    private void EndBattle(bool hasWon)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        freeRoamCamera.gameObject.SetActive(true);
    }

    private void OnShowDialogue()
    {
        state = GameState.Dialogue;
    }

    private void OnCloseDialogue()
    {
        if (state == GameState.Dialogue)
            state = GameState.FreeRoam;
    }
}