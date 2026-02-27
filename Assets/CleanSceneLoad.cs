using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanSceneLoad : MonoBehaviour
{
    public string sceneToReload;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ReloadScene(sceneToReload));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator ReloadScene(string scene)
    {
        Scene targetScene = SceneManager.GetSceneByName(scene);
        SceneManager.UnloadSceneAsync(targetScene);
        yield return new WaitUntil(() => (!targetScene.isLoaded));
        SceneManager.LoadScene(scene);
    }
}
