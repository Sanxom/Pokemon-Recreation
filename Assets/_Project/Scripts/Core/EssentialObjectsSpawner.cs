using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectPrefab;

    private void Awake()
    {
        EssentialObjects[] existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            Instantiate(essentialObjectPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}