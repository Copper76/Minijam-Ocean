using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    private Scene nextScene;
    private AsyncOperation asyncLoad;

    public void NextLevel()
    {
        LoadLevel(nextLevelName);
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(NextLevel(levelName));
    }

    IEnumerator NextLevel(string levelName)
    {
        nextScene = SceneManager.GetSceneByName(levelName);
        asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }
}
