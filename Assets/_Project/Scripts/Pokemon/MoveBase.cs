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
    [SerializeField] private Category category1;
    [SerializeField] private Category category2;

    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int startingPP;
    [SerializeField] private int maxPossiblePP;

    public PokemonType Type => type;
    public Category Category1 => category1;
    public Category Category2 => category2;
    public string MoveName => moveName;
    public string Description => description;
    public int Power => power;
    public int Accuracy => accuracy;
    public int StartingPP => startingPP;
    public int MaxPossiblePP => maxPossiblePP;
    public bool IsSpecial
    {
        get
        {
            if (category1 == Category.Special)
                return true;

            return false;
        }
    }
}