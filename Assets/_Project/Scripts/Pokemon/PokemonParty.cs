using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemonList;

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
}