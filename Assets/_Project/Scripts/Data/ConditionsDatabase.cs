using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionID
{
    None,
    Poison,
    SuperPoison,
    Burn,
    Sleep,
    Paralyze,
    Freeze
}

public class ConditionsDatabase
{
    public static Dictionary<ConditionID, Condition> ConditionsDictionary { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Poison,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned.",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHealth(pokemon.MaxHealth / 8);
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is hurt by poison.");
                }
            }
        },

        {
            ConditionID.SuperPoison,
            new Condition()
            {
                Name = "Super Poison",
                StartMessage = "has been badly poisoned.",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    int x = Mathf.FloorToInt((float)pokemon.MaxHealth / 16);
                    if (x < 1)
                        x = 1;
                    int damage = pokemon.SuperPoisonIncrement * x;
                    pokemon.UpdateHealth(damage);
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is hurt by poison.");
                    pokemon.SuperPoisonIncrement++;
                }
            }
        },

        {
            ConditionID.Burn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned.",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHealth(pokemon.MaxHealth / 16);
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is hurt by burn.");
                }
            }
        },

        {
            ConditionID.Sleep,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep.",
                OnStart= (Pokemon pokemon) =>
                {
                    // Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"{pokemon.PokemonBase.PokemonName} will be asleep for {pokemon.StatusTime} turns.");
                },

                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is sleeping.");

                    return false;
                }
            }
        },

        {
            ConditionID.Paralyze,
            new Condition()
            {
                Name = "Paralyze",
                StartMessage = "has been paralyzed.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is fully paralyzed.");
                        return false;
                    }

                    return true;
                }
            }
        },

        {
            ConditionID.Freeze,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen.",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} thawed out!");
                        return true;
                    }

                    return false;
                }
            }
        },
    };
}