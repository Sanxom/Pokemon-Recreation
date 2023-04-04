using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DestinationIdentifier
{
    A,
    B,
    C,
    D,
    E
}

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;
    [SerializeField] private int sceneToLoad = -1;

    private PlayerController player;

    public Transform SpawnPoint => spawnPoint;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameManager.Instance.PauseGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        Portal destinationPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        GameManager.Instance.PauseGame(false);

        Destroy(gameObject);
    }
}