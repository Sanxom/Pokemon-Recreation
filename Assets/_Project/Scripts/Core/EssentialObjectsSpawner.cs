using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();

        if (existingObjects.Length == 0)
        {
            // If there is a Grid, we should spawn at its center
            Vector3 spawnPos = Vector3.zero;

            Grid grid = FindObjectOfType<Grid>();
            if (grid != null )
            {
                spawnPos = grid.transform.position;
            }

            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}