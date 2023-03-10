using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase pokemonBase;
    [SerializeField] private int level;

    private const int minimumBoostAmount = -6;
    private const int maximumBoostAmount = 6;

    // Properties
    public Dictionary<Stat, int> StatDictionary { get; private set; }
    public Dictionary<Stat, int> StatBoostDictionary { get; private set; }
    public Queue<string> StatusChangeQueue { get; private set; } = new Queue<string>();
    public List<Move> MoveList { get; set; }
    public PokemonBase PokemonBase => pokemonBase;
    public int Level => level;
    public int Health { get; set; }
    public int MaxHealth => GetStatWithBoost(Stat.MaxHealth);
    public int Attack => GetStatWithBoost(Stat.Attack);
    public int Defense => GetStatWithBoost(Stat.Defense);
    public int SpAttack => GetStatWithBoost(Stat.SpAttack);
    public int SpDefense => GetStatWithBoost(Stat.SpDefense);
    public int Speed => GetStatWithBoost(Stat.Speed);

    public void Init()
    {
        MoveList = new List<Move>();

        // Generate moves
        foreach (var move in pokemonBase.LearnableMoves)
        {
            if (move.LevelLearned <= level)
                MoveList.Add(new Move(move.MoveBase));

            if (MoveList.Count >= 4)
                break;
        }

        CalculateStats();

        ResetStatBoosts();

        Health = MaxHealth;
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

        float attack = move.Base.Category1 == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.Category1 == MoveCategory.Special ? SpDefense : Defense;

        int damage;
        float modifiers = Random.Range(0.85f, 1f) * type * critical * stab;
        float a = (2 * attacker.Level / 5 + 2);
        float d;

        d = (a * move.Base.Power * ((float)attack / defense)) / 50 + 2;
        damage = Mathf.FloorToInt(d * modifiers);
        Health -= damage;

        if (Health <= 0)
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

    public void ApplyBoosts(List<StatBoost> statBoostList, BattleUnit battleUnit)
    {
        foreach (StatBoost statBoost in statBoostList)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;

            StatBoostDictionary[stat] = Mathf.Clamp(StatBoostDictionary[stat] + boost, minimumBoostAmount, maximumBoostAmount);

            if (battleUnit.IsPlayerUnit)
            {
                if (boost > 0)
                    StatusChangeQueue.Enqueue($"{pokemonBase.PokemonName}'s {stat} rose!");
                else
                    StatusChangeQueue.Enqueue($"{PokemonBase.PokemonName}'s {stat} fell!");
            }
            else
            {
                if (boost > 0)
                    StatusChangeQueue.Enqueue($"The enemy {pokemonBase.PokemonName}'s {stat} rose!");
                else
                    StatusChangeQueue.Enqueue($"The enemy {pokemonBase.PokemonName}'s {stat} fell!");
            }
        }
    }

    public void OnBattleOver()
    {
        ResetStatBoosts();
    }

    private int GetStatWithBoost(Stat stat)
    {
        int statValue = StatDictionary[stat];

        // Apply stat boost
        float[] boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        int boost = StatBoostDictionary[stat];

        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    private void CalculateStats()
    {
        StatDictionary = new Dictionary<Stat, int>
        {
            { Stat.MaxHealth, Mathf.FloorToInt(PokemonBase.MaxHealth * Level / 100f) + 10 },
            { Stat.Attack, Mathf.FloorToInt(PokemonBase.Attack * Level / 100f) + 5 },
            { Stat.Defense, Mathf.FloorToInt(PokemonBase.Defense * Level / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt(PokemonBase.SpAttack * Level / 100f) + 5 },
            { Stat.SpDefense, Mathf.FloorToInt(PokemonBase.SpDefense * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(PokemonBase.Speed * Level / 100f) + 5 }
        };
    }

    private void ResetStatBoosts()
    {
        StatBoostDictionary = new Dictionary<Stat, int>()
        {
            { Stat.MaxHealth, 0 },
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 }
        };
    }
}

public class DamageDetails
{
    public float TypeEffectiveness { get; set; }
    public float Critical { get; set; }
    public bool Fainted { get; set; }
}