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

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        // Critical Hit
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        // STAB bonus
        float stab = 1f;
        if (move.Base.Type == attacker.PokemonBase.Type1 || move.Base.Type == attacker.PokemonBase.Type2)
            stab = 1.5f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, PokemonBase.Type1) * TypeChart.GetEffectiveness(move.Base.Type, PokemonBase.Type2);

        DamageDetails damageDetails = new()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = move.Base.IsSpecial ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.IsSpecial ? SpDefense : Defense;

        int damage;
        float modifiers = Random.Range(0.85f, 1f) * type * critical * stab;
        float a = (2 * attacker.Level / 5 + 2);
        float d;
        // TODO: Change this to account for Physical and Special Attack/Defense
        // TODO: Also refactor to remove these if/elses?
        if (move.Base.Category1 == Category.Status)
        {
            // Apply status effect here?
        }
        else if (move.Base.Category2 == Category.Status)
        {
            d = (a * move.Base.Power * ((float)attack / defense)) / 50 + 2;
            damage = Mathf.FloorToInt(d * modifiers);
            Health -= damage;
            // Apply status effect here?
        }
        else if (move.Base.Category1 != Category.Status && move.Base.Category2 != Category.Status)
        {
            d = (a * move.Base.Power * ((float)attack / defense)) / 50 + 2;
            damage = Mathf.FloorToInt(d * modifiers);
            Health -= damage;
        }

        if(Health <= 0)
        {
            Health = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int randomMove = Random.Range(0, MoveList.Count);
        return MoveList[randomMove];
    }
}

public class DamageDetails
{
    public float TypeEffectiveness { get; set; }
    public float Critical { get; set; }
    public bool Fainted { get; set; }
}