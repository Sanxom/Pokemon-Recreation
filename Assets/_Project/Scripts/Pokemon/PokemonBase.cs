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

    // Properties
    public string PokemonName 
    { 
        get 
        { 
            return pokemonName; 
        } 
    }
    public string Description
    {
        get
        {
            return description;
        }
    }
    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }
    public int Attack
    {
        get
        {
            return attack;
        }
    }
    public int Defense
    {
        get
        {
            return defense;
        }
    }
    public int SpAttack
    {
        get
        {
            return spAttack;
        }
    }
    public int SpDefense
    {
        get
        {
            return spDefense;
        }
    }
    public int Speed
    {
        get
        {
            return speed;
        }
    }
}