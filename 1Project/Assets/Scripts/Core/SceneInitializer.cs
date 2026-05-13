using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private string sceneType; // "MainMenu" или "Gameplay"

    private void Start()
    {
        var bootstrapper = FindObjectOfType<Bootstrapper>();
        if (bootstrapper == null)
        {
            Debug.LogError("Bootstrapper не найден!");
            return;
        }

        var services = bootstrapper.GetServices();
        var entryPointPrefab = bootstrapper.GetEntryPointPrefab(sceneType);

        if (entryPointPrefab != null)
        {
            var entryPointObj = Instantiate(entryPointPrefab);
            var entryPoint = entryPointObj.GetComponent<EntryPoint>();
            entryPoint.Initialize(services);
            entryPoint.Run();
        }
    }
}