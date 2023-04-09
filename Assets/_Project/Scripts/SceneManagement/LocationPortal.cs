using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;

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
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameManager.Instance.PauseGame(true);
        yield return fader.FadeToBlack(fadeDuration);

        LocationPortal destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeIn(fadeDuration);
        GameManager.Instance.PauseGame(false);
    }
}