using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private HealthBar healthBar;

    private Pokemon pokemon;

    public void SetData(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        nameText.text = pokemon.PokemonBase.PokemonName;
        levelText.text = "Lv" + pokemon.Level;
        healthBar.SetHealth((float)pokemon.Health / pokemon.MaxHealth);
    }

    public IEnumerator UpdateHealth()
    {
        yield return healthBar.SetHealthSmooth((float)pokemon.Health / pokemon.MaxHealth);
    }
}