using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Category
{
    None,
    Physical,
    Special,
    Status
}

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private PokemonType type;
    [SerializeField] private Category category;

    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int startingPP;
    [SerializeField] private int maxPossiblePP;

    public PokemonType Type { get => type; }
    public Category Category { get => category; }
    public string MoveName { get => moveName; }
    public string Description { get => description; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int StartingPP { get => startingPP; }
    public int MaxPossiblePP { get => maxPossiblePP; }
}