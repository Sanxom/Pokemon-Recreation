using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    // Properties
    public PokemonBase PokemonBase { get; set; }
    public List<Move> Moves { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }
    public int MaxHealth => Mathf.FloorToInt((PokemonBase.MaxHealth * Level) / 100f) + 10;
    public int Attack => Mathf.FloorToInt((PokemonBase.Attack * Level) / 100f) + 5;
    public int Defense => Mathf.FloorToInt((PokemonBase.Defense * Level) / 100f) + 5;
    public int SpAttack => Mathf.FloorToInt((PokemonBase.SpAttack * Level) / 100f) + 5;
    public int SpDefense => Mathf.FloorToInt((PokemonBase.SpDefense * Level) / 100f) + 5;
    public int Speed => Mathf.FloorToInt((PokemonBase.Speed * Level) / 100f) + 5;

    public Pokemon(PokemonBase pokemonBase, int level)
    {
        PokemonBase = pokemonBase;
        Level = level;
        Health = MaxHealth;
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