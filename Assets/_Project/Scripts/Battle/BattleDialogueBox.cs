using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject actionSelectorGO;
    [SerializeField] private GameObject moveSelectorGO;
    [SerializeField] private GameObject moveDetailsGO;
    [SerializeField] private GameObject choiceBoxGO;
    [SerializeField] private Color highlightedColor;

    [SerializeField] private List<TextMeshProUGUI> actionTextList;
    [SerializeField] private List<TextMeshProUGUI> moveTextList;

    [SerializeField] private TextMeshProUGUI ppText;
    [SerializeField] private TextMeshProUGUI moveTypeText;
    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private int lettersPerSecond;

    private WaitForSeconds letterAnimationRoutineDelay;
    private WaitForSeconds dialogueEndRoutineDelay;
    private float dialogueEndDelay = 1f;
    private float letterAnimationDelay;

    private void Start()
    {
        letterAnimationDelay = 1f / lettersPerSecond;
        letterAnimationRoutineDelay = new WaitForSeconds(letterAnimationDelay);
        dialogueEndRoutineDelay = new WaitForSeconds(dialogueEndDelay);
    }

    public void SetDialogue(string dialogueText)
    {
        this.dialogueText.text = dialogueText;
    }

    public IEnumerator TypeDialogue(string dialogueText)
    {
        this.dialogueText.text = "";
        foreach (var letter in dialogueText.ToCharArray())
        {
            this.dialogueText.text += letter;
            yield return letterAnimationRoutineDelay;
        }

        yield return dialogueEndRoutineDelay;
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelectorGO.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelectorGO.SetActive(enabled);
        moveDetailsGO.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBoxGO.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTextList.Count; i++)
        {
            if (i == selectedAction)
                actionTextList[i].color = highlightedColor;
            else
                actionTextList[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(Move move, int selectedMove)
    {
        for (int i = 0; i < moveTextList.Count; i++)
        {
            if (i == selectedMove)
                moveTextList[i].color = highlightedColor;
            else
                moveTextList[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.StartingPP}";
        moveTypeText.text = move.Base.Type.ToString();

        if (move.PP <= 0)
            ppText.color = Color.red;
        else if (move.PP <= move.Base.StartingPP * 0.5f)
            ppText.color = new Color(1f, 0.647f, 0f);
        else
            ppText.color = Color.black;
    }

    public void UpdateChoiceBox(bool isYesSelected)
    {
        if (isYesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            noText.color = highlightedColor;
            yesText.color = Color.black;
        }
    }

    public void SetMoveNames(List<Move> moveList)
    {
        for (int i = 0; i < moveTextList.Count; i++)
        {
            if (i < moveList.Count)
                moveTextList[i].text = moveList[i].Base.MoveName;
            else
                moveTextList[i].text = "-";
        }
    }
}