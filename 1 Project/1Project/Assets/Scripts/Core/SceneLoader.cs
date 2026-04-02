using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : IService
{
    public void Initialize() { }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName, System.Action onComplete = null)
    {
        SceneManager.LoadSceneAsync(sceneName).completed += (op) =>
        {
            onComplete?.Invoke();
        };
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cleanup() { }
}