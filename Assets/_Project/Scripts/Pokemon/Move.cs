using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int StartingPP { get; set; }

    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        StartingPP = moveBase.StartingPP;
    }
}