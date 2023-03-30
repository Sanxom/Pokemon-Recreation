using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    private PartyMemberUI[] memberSlotsList;
    private List<Pokemon> pokemonList;

    public void Init()
    {
        memberSlotsList = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Pokemon> pokemonList)
    {
        this.pokemonList = pokemonList;

        for (int i = 0; i < memberSlotsList.Length; i++)
        {
            if (i < pokemonList.Count)
            {
                memberSlotsList[i].gameObject.SetActive(true);
                memberSlotsList[i].SetData(pokemonList[i]);
            }
            else
                memberSlotsList[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon.";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemonList.Count; i++)
        {
            if (i == selectedMember)
                memberSlotsList[i].SetSelected(true);
            else
                memberSlotsList[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}