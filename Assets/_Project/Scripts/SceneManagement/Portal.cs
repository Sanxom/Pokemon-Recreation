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
    private Fader fader;
    private float fadeDuration = 0.5f;

    public Transform SpawnPoint => spawnPoint;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameManager.Instance.PauseGame(true);
        yield return fader.FadeToBlack(fadeDuration);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeIn(fadeDuration);
        GameManager.Instance.PauseGame(false);

        Destroy(gameObject);
    }
}