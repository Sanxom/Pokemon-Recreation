using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> moveTextList;
    [SerializeField] private Color highlightedColor;

    private int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoveList, MoveBase newMove)
    {
        for (int i = 0; i < currentMoveList.Count; i++)
        {
            moveTextList[i].text = currentMoveList[i].MoveName;
        }

        moveTextList[currentMoveList.Count].text = "Don't Learn";
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < PokemonBase.MaxNumOfMoves + 1; i++)
        {
            if (i == selection)
                moveTextList[i].color = highlightedColor;
            else
                moveTextList[i].color = Color.black;
        }
    }

    public void HandleMoveSelection(Action<int> OnSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ++currentSelection;
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.E))
            OnSelected?.Invoke(currentSelection);
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnSelected?.Invoke(PokemonBase.MaxNumOfMoves);
        }
    }
}