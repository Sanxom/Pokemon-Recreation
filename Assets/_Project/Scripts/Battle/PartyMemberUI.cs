using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Color highlightedColor;

    private Pokemon pokemon;

    public void SetData(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        nameText.text = pokemon.PokemonBase.PokemonName;
        levelText.text = "Lv" + pokemon.Level;
        healthBar.SetHealth((float)pokemon.Health / pokemon.MaxHealth);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}