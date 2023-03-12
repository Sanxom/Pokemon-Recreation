using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> wildPokemonList;

    public Pokemon GetRandomWildPokemon()
    {
        // TODO: Base this on rarity of Pokemon instead of just getting a random one
        Pokemon wildPokemon = wildPokemonList[Random.Range(0, wildPokemonList.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}