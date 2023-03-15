using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea]
    [SerializeField] private List<string> linesList;

    public List<string> LinesList => linesList;
}