using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        print("Interacting with NPC.");
    }
}