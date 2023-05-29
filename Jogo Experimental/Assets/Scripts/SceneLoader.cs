using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject loaderCanvas;

    private List<int> sceneIndices = new List<int>();
    private int currentSceneIndex = 0;
    private bool isPlaying = false;

    private void Start()
    {
        loaderCanvas.SetActive(false);
        InitializeSceneIndices();
        LoadScene(0); // Load the menu scene (index 0) first
    }

    private void InitializeSceneIndices()
    {
        // Add level scene build indices (1 and 2) to the list
        sceneIndices.Add(1);
        sceneIndices.Add(2);
    }

    public void PlayGame()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        if (currentSceneIndex < sceneIndices.Count)
        {
            int sceneIndex = sceneIndices[currentSceneIndex];
            currentSceneIndex++;

            LoadScene(sceneIndex);
        }
        else
        {
            EndGame();
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loaderCanvas.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            Debug.Log("Loading: " + progress);
            yield return null;
        }

        // If the loaded scene is a level scene (not the menu), load the next scene after a short delay
        if (sceneIndex > 0)
        {
            yield return new WaitForSeconds(1f);
            LoadNextScene();
        }
    }

    private void EndGame()
    {
        // Implement your game-ending logic here
        Debug.Log("Game ended");
        // You can add code to display a game over screen, show scores, etc.
    }

    private void OnDestroy()
    {
        // Save the current scene index before being destroyed
        PlayerPrefs.SetInt("PreviousSceneIndex", currentSceneIndex);
    }
}
