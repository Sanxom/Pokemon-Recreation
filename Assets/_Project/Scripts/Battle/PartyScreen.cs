using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    private PartyMemberUI[] memberSlotsList;

    public void Init()
    {
        memberSlotsList = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemonList)
    {
        for (int i = 0; i < memberSlotsList.Length; i++)
        {
            if (i < pokemonList.Count)
                memberSlotsList[i].SetData(pokemonList[i]);
            else
                memberSlotsList[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon.";
    }
}