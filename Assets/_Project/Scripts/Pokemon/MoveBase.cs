using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveCategory
{
    None,
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe,
    Self
}

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] MoveEffects effects;

    [SerializeField] private PokemonType type;
    [SerializeField] private MoveCategory category1;
    [SerializeField] private MoveCategory category2;
    [SerializeField] private MoveTarget target;

    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int startingPP;
    [SerializeField] private int maxPossiblePP;

    public MoveEffects Effects => effects;
    public PokemonType Type => type;
    public MoveCategory Category1 => category1;
    public MoveCategory Category2 => category2;
    public MoveTarget Target => target;
    public string MoveName => moveName;
    public string Description => description;
    public int Power => power;
    public int Accuracy => accuracy;
    public int StartingPP => startingPP;
    public int MaxPossiblePP => maxPossiblePP;
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boostList;

    public List<StatBoost> BoostList => boostList;
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}