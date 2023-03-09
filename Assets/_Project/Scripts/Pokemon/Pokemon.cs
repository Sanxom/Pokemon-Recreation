using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    private PokemonBase pokemonBase;
    private int level;

    // Properties
    public List<Move> Moves { get; set; }
    public int StartingHealth { get; set; }
    public int MaxHealth
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.MaxHealth * level) / 100f) + 10;
        }
    }
    public int Attack
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.Attack * level) / 100f) + 5;
        }
    }
    public int Defense
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.Defense * level) / 100f) + 5;
        }
    }
    public int SpAttack
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.SpAttack * level) / 100f) + 5;
        }
    }
    public int SpDefense
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.SpDefense * level) / 100f) + 5;
        }
    }
    public int Speed
    {
        get
        {
            return Mathf.FloorToInt((pokemonBase.Speed * level) / 100f) + 5;
        }
    }

    public Pokemon(PokemonBase pokemonBase, int level)
    {
        this.pokemonBase = pokemonBase;
        this.level = level;
        StartingHealth = pokemonBase.MaxHealth;
        Moves = new List<Move>();

        // Generate moves
        foreach (var move in pokemonBase.LearnableMoves)
        {
            if (move.LevelLearned <= level)
                Moves.Add(new Move(move.MoveBase));

            if (Moves.Count >= 4)
                break;
        }
    }
}