using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dark,
    Dragon,
    Steel,
    Fairy
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

    [Header("Base Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    [SerializeField] private List<LearnableMoves> learnableMoves;

    // Properties
    public List<LearnableMoves> LearnableMoves => learnableMoves;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public string PokemonName => pokemonName;
    public string Description => description;
    public int MaxHealth => maxHealth;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int levelLearned;

    public MoveBase MoveBase { get => moveBase; }
    public int LevelLearned { get => levelLearned; }
}