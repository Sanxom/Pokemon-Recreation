using DG.Tweening;
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
    [SerializeField] private GameObject xpBar;

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
        SetLevel();
        healthBar.SetHealth((float)pokemon.Health / pokemon.MaxHealth);
        SetXPBarValue();

        statusColorsDictionary = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.PSN, poisonColor },
            { ConditionID.PSN2, poisonColor },
            { ConditionID.BRN, burnColor },
            { ConditionID.SLP, sleepColor },
            { ConditionID.PAR, paralyzeColor },
            { ConditionID.FRZ, freezeColor }
        };

        SetStatusText();
        pokemon.OnStatusChanged += SetStatusText;
    }

    public void SetLevel()
    {
        levelText.text = "Lv" + pokemon.Level;
    }

    public void SetXPBarValue()
    {
        if (xpBar == null)
            return;

        float normalizedXP = GetNormalizedXP();
        xpBar.transform.localScale = new Vector3(normalizedXP, 1, 1);
    }

    public IEnumerator SetXPBarValueSmooth(bool reset = false)
    {
        if (xpBar == null)
            yield break;

        if (reset)
            xpBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedXP = GetNormalizedXP();
        yield return xpBar.transform.DOScaleX(normalizedXP, 1.5f).WaitForCompletion();
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

    private float GetNormalizedXP()
    {
        int currentLevelXP = pokemon.PokemonBase.GetXPForLevel(pokemon.Level);
        int nextLevelXP = pokemon.PokemonBase.GetXPForLevel(pokemon.Level + 1);

        float normalizedXP = (float)(pokemon.XP - currentLevelXP) / (nextLevelXP - currentLevelXP);
        return Mathf.Clamp01(normalizedXP);
    }
}