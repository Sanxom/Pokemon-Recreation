using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private HealthBar healthBar;

    [SerializeField] private Color poisonColor;
    [SerializeField] private Color burnColor;
    [SerializeField] private Color sleepColor;
    [SerializeField] private Color paralyzeColor;
    [SerializeField] private Color freezeColor;

    private Dictionary<ConditionID, Color> statusColorsDictionary;
    private Pokemon pokemon;

    public IEnumerator UpdateHealth()
    {
        if (pokemon.HasHealthChanged)
        {
            yield return healthBar.SetHealthSmooth((float)pokemon.Health / pokemon.MaxHealth);
            pokemon.HasHealthChanged = false;
        }
    }

    public void SetData(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        nameText.text = pokemon.PokemonBase.PokemonName;
        levelText.text = "Lv" + pokemon.Level;
        healthBar.SetHealth((float)pokemon.Health / pokemon.MaxHealth);

        statusColorsDictionary = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.PSN, poisonColor},
            {ConditionID.PSN2, poisonColor },
            {ConditionID.BRN, burnColor},
            {ConditionID.SLP, sleepColor},
            {ConditionID.PAR, paralyzeColor },
            {ConditionID.FRZ, freezeColor}
        };

        SetStatusText();
        pokemon.OnStatusChanged += SetStatusText;
    }

    private void SetStatusText()
    {
        if (pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColorsDictionary[pokemon.Status.Id];
        }
    }
}