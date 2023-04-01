using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    MaxHealth,
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // These 2 are not actual stats, they are used to alter moveAccuracy
    Accuracy,
    Evasion
}

public enum GrowthRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating
}

public class TypeChart
{
    static float[][] chart =
    {             
                              //NOR   FIR   WAT   ELE   GRA   ICE   FIG   POI   GRO   FLY   PSY   BUG   ROC   GHO   DRA   DAR   STE   FAI
        /*NORMAL*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0,    1f,   1f,   0.5f, 1f },
        /*FIRE*/   new float[] {1f,   0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 1f,   2f,   1f },
        /*WATER*/  new float[] {1f,   2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   2f,   1f },
        /*ELEC*/   new float[] {1f,   1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f },
        /*GRASS*/  new float[] {1f,   0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 1f,   0.5f, 1f},
        /*ICE*/    new float[] {1f,   0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f },
        /*FIGHT*/  new float[] {2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f,   2f,   2f,   0.5f },
        /*POIS*/   new float[] {1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   0f,   2f },
        /*GROUND*/ new float[] {1f,   2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f,   1f,   2f,   1f },
        /*FLYING*/ new float[] {1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 1f },
        /*PSYCH*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f,   0f,   0.5f, 1f },
        /*BUG*/    new float[] {1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   1f,   0.5f, 1f,   2f,   0.5f, 0.5f },
        /*ROCK*/   new float[] {1f,   2f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f },
        /*GHOST*/  new float[] {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   1f },
        /*DRAGON*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 0f },
        /*DARK*/   new float[] {1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f },
        /*STEEL*/  new float[] {1f,   0.5f, 0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   0.5f, 2f },
        /*FAIRY*/  new float[] {1f,   0.5f, 1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   0.5f, 1f }
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1f;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create New Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private string pokemonName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    [SerializeField] private GrowthRate growthRate;

    [Header("Base Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    [SerializeField] private int xpYield;
    [SerializeField] private int catchRate = 255;

    [SerializeField] private List<LearnableMoves> learnableMoves;

    // Properties
    public List<LearnableMoves> LearnableMoves => learnableMoves;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;
    public string PokemonName => pokemonName;
    public string Description => description;
    public int MaxHealth => maxHealth;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;
    public int XpYield => xpYield;
    public GrowthRate GrowthRate => growthRate;
    public int CatchRate => catchRate;

    public int GetXPForLevel(int level)
    {
        // Growth Rate formulas
        if (growthRate == GrowthRate.Erratic)
        {
            if (level < 50)
            {
                return level * level * level * (100 - level) / 50;
            }
            else if (level < 68)
            {
                return level * level * level * (150 - level) / 100;
            }
            else if (level < 98)
            {
                return level * level * level * (1911 - (10 * level) / 3);
            }
            else if (level < 100)
            {
                return level * level * level * (160 - level) / 100;
            }
        }
        else if (growthRate == GrowthRate.Fast)
        {
            return 4 * level * level * level / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return (6 / 5 * level * level * level) - (15 * level * level) + (100 * level) - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * level * level * level / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            if (level < 15)
            {
                return level * level * level * (((level + 1) / 3) + 24) / 50;
            }
            else if (level < 36)
            {
                return level * level * level * (level + 14) / 50;
            }
            else if (level < 100)
            {
                return level * level * level * ((level / 2) + 32) / 50;
            }
        }

        return -1;
    }
}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int levelLearned;

    public MoveBase MoveBase { get => moveBase; }
    public int LevelLearned { get => levelLearned; }
}