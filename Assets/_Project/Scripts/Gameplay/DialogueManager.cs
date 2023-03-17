using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private int lettersPerSecond;

    private Action OnDialogueFinished;
    private Dialogue dialogue;
    private WaitForSeconds letterAnimationRoutineDelay;
    private float letterAnimationDelay;
    private int currentLine;
    private bool isTyping;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        letterAnimationDelay = 1f / lettersPerSecond;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue, Action OnDialogueFinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        IsShowing = true;
        this.dialogue = dialogue;
        this.OnDialogueFinished = OnDialogueFinished;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.LinesList[0]));
    }

    public IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return letterAnimationRoutineDelay;
        }
        isTyping = false;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.LinesList.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.LinesList[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogueBox.SetActive(false);
                OnDialogueFinished?.Invoke();
                OnCloseDialogue?.Invoke();
            }
        }
    }
}