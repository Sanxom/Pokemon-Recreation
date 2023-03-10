using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    // Properties
    public PokemonBase PokemonBase { get; set; }
    public List<Move> MoveList { get; set; }
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
        MoveList = new List<Move>();

        // Generate moves
        foreach (var move in pokemonBase.LearnableMoves)
        {
            if (move.LevelLearned <= level)
                MoveList.Add(new Move(move.MoveBase));

            if (MoveList.Count >= 4)
                break;
        }
    }

    public bool TakeDamage(Move move, Pokemon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level / 5 + 2);
        float d = (a * move.Base.Power * ((float)attacker.Attack / Defense)) / 50 + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        Health -= damage;

        if(Health <= 0)
        {
            Health = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int randomMove = Random.Range(0, MoveList.Count);
        return MoveList[randomMove];
    }
}