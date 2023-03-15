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
    [SerializeField] List<SecondaryEffects> secondaryEffectsList;
    [SerializeField] MoveEffects effects;

    [SerializeField] private PokemonType type;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveTarget target;

    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int startingPP;
    [SerializeField] private int maxPossiblePP;
    [SerializeField] private bool alwaysHits;

    public List<SecondaryEffects> SecondaryEffectsList => secondaryEffectsList;
    public MoveEffects Effects => effects;
    public PokemonType Type => type;
    public MoveCategory Category => category;
    public MoveTarget Target => target;
    public string MoveName => moveName;
    public string Description => description;
    public int Power => power;
    public int Accuracy => accuracy;
    public int StartingPP => startingPP;
    public int MaxPossiblePP => maxPossiblePP;
    public bool AlwaysHits => alwaysHits;
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boostList;
    [SerializeField] private ConditionID status;
    [SerializeField] private ConditionID volatileStatus;

    public List<StatBoost> BoostList => boostList;
    public ConditionID Status => status;
    public ConditionID VolatileStatus => volatileStatus;
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] private MoveTarget target;
    [SerializeField] private int chance;

    public MoveTarget Target => target;
    public int Chance => chance;
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}