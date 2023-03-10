using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject health;

    public void SetHealth(float healthNormalized)
    {
        health.transform.localScale = new Vector3(healthNormalized, 1f);
    }

    public IEnumerator SetHealthSmooth(float newHealth)
    {
        float currentHealth = health.transform.localScale.x;
        float changeAmount = currentHealth - newHealth;

        while (currentHealth - newHealth > Mathf.Epsilon)
        {
            currentHealth -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHealth, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHealth, 1f);
    }
}