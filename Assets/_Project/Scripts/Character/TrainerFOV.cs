using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameManager.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}