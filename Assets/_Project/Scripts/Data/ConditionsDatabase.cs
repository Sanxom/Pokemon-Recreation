using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionID
{
    None,
    PSN,
    PSN2,
    BRN,
    SLP,
    PAR,
    FRZ,
    Confusion
}

public class ConditionsDatabase
{
    public static void Init()
    {
        foreach (KeyValuePair<ConditionID, Condition> kvp in ConditionsDictionary)
        {
            ConditionID conditionId = kvp.Key;
            Condition condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> ConditionsDictionary { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        // Status Conditions
        {
            ConditionID.PSN,
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
            ConditionID.PSN2,
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
            ConditionID.BRN,
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
            ConditionID.SLP,
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
            ConditionID.PAR,
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
            ConditionID.FRZ,
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

        // Volatile Status Conditions
        {
            ConditionID.Confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused.",
                OnStart= (Pokemon pokemon) =>
                {
                    // Confused for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"{pokemon.PokemonBase.PokemonName} will be confused for {pokemon.VolatileStatusTime} turns.");
                },

                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} snapped out of confusion!");
                        return true;
                    }

                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} is confused.");
                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    // Hurt by confusion
                    pokemon.UpdateHealth(pokemon.MaxHealth / 8);
                    pokemon.StatusChangeQueue.Enqueue($"{pokemon.PokemonBase.PokemonName} hurt itself in its confusion.");
                    return false;
                }
            }
        }
    };
}