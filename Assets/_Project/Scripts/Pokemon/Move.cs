using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }
    public int MaxPossiblePP { get; set; }

    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = moveBase.StartingPP;
        MaxPossiblePP = moveBase.MaxPossiblePP;
    }
}