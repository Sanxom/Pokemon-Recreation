using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers Instance { get; set; }

    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask interactableLayer;

    public LayerMask SolidObjectsLayer => solidObjectsLayer;
    public LayerMask GrassLayer => grassLayer;
    public LayerMask InteractableLayer => interactableLayer;

    private void Awake()
    {
        Instance = this;
    }
}