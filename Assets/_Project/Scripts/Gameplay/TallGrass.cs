using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TallGrass : MonoBehaviour, IPlayerTriggerable
{
    private int minEncounterRate = 1;
    private int maxEncounterRate = 101;
    private int encounterPercentage = 10;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (Random.Range(minEncounterRate, maxEncounterRate) <= encounterPercentage)
        {
            player.Character.Animator.IsMoving = false;
            GameManager.Instance.StartWildBattle();
        }
    }
}