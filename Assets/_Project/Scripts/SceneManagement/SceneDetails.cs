using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> connectedScenesList;

    public bool IsLoaded { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print($"Entered {gameObject.name}");

            LoadScene();
            GameManager.Instance.SetCurrentScene(this);

            foreach (SceneDetails scene in connectedScenesList)
            {
                // Load all connected Scenes
                scene.LoadScene();
            }

            if (GameManager.Instance.PreviousScene != null)
            {
                // Unload the Scenes that are no longer connected
                List<SceneDetails> previouslyLoadedScenes = GameManager.Instance.PreviousScene.connectedScenesList;
                foreach (SceneDetails scene in previouslyLoadedScenes)
                {
                    if (!connectedScenesList.Contains(scene) && scene != this)
                    {
                        print("Hello");
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}