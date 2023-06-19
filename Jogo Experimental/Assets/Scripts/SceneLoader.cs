using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public int currentScene;

    [SerializeField] Slider slider;
    [SerializeField] GameObject loaderCanvas;
    public List<int> sceneBuildIndices = new List<int>(); // Indices das cenas, sem o menu
    public static List<int> sceneBuildIndicesRandomized = new List<int>(); // Indices das cenas randomizados

    void Start() 
    {
        loaderCanvas.SetActive(false);

        if (sceneBuildIndicesRandomized.Count == 0)
        {
            List<int> sceneBuildIndices = GetAllSceneBuildIndices();
            sceneBuildIndicesRandomized = ShuffleSceneIndices(sceneBuildIndices);
        /*
            Logger.Instance.LogAction("Scene Build Indices in the Project:");
            foreach (int buildIndex in sceneBuildIndicesRandomized)
            {
                Logger.Instance.LogAction("- Build Index: " + buildIndex);
            } 
        */
        }        
    }

    private List<int> GetAllSceneBuildIndices()
    {
        // Creates a List with scenes indices, except the menu
        
        List<int> sceneBuildIndices = new List<int>();

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (!string.IsNullOrEmpty(scenePath))
            {
                sceneBuildIndices.Add(i);
            }
        }

        return sceneBuildIndices;
    }

    private static List<int> ShuffleSceneIndices(List<int> sceneIndices)
    {
        // Fisher-Yates shuffle algorithm.

        int n = sceneIndices.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = sceneIndices[k];
            sceneIndices[k] = sceneIndices[n];
            sceneIndices[n] = value;
        }

        return sceneIndices;
    }
    
    
    public void LoadSceneByPositionOnList(int currentSceneIndex)
    {
        int n = sceneBuildIndicesRandomized.IndexOf(currentSceneIndex);
        Debug.Log("n is " + n);

        if (currentSceneIndex == 0)
        {
            LoadScene(sceneBuildIndicesRandomized[0]);
            Logger.Instance.LogAction("Menu --> " + sceneBuildIndicesRandomized[0]);
        }
        else if (n >= 0 && n < sceneBuildIndicesRandomized.Count - 1)
        {
            int nextSceneIndex = sceneBuildIndicesRandomized[n + 1];
            LoadScene(nextSceneIndex);
            Logger.Instance.LogAction(sceneBuildIndicesRandomized[n] + " --> " + nextSceneIndex);
        }
        else if (n == sceneBuildIndicesRandomized.Count - 1)
        {
            Logger.Instance.LogAction("Game Finalizado. Todas as Scenas foram jogadas.");
            LoadScene(0);
        }
    }
    
    
    
    public void LoadScene (int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loaderCanvas.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            slider.value = progress;
            yield return null;
        }
    }

}