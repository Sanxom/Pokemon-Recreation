using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemonList;

    public List<Pokemon> PokemonList => pokemonList;

    private void Start()
    {
        foreach (Pokemon pokemon in pokemonList)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemonList.Where(x => x.Health > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemonList.Count < 6)
            pokemonList.Add(newPokemon);
        else
        {
            // TODO: Transfer to PC
        }
    }
}