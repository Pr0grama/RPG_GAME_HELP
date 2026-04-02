using UnityEngine;

public class MainMenuEntryPoint : EntryPoint
{
    [SerializeField] private MainMenuView mainMenuViewPrefab; // Префаб MainMenuCanvas
    private MainMenuView mainMenuView;
    private MainMenuController controller;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        // СОЗДАЕМ МЕНЮ ИЗ ПРЕФАБА
        if (mainMenuViewPrefab != null)
        {
            var canvasInstance = Instantiate(mainMenuViewPrefab);
            mainMenuView = canvasInstance.GetComponent<MainMenuView>();
            Debug.Log("✅ MainMenuCanvas created from prefab");
        }
        else
        {
            Debug.LogError("❌ mainMenuViewPrefab is NULL! Assign it in the inspector!");
            return;
        }

        if (mainMenuView == null)
        {
            Debug.LogError("❌ MainMenuView component not found on prefab!");
            return;
        }

        var audioService = services.Get<AudioService>();
        var sceneLoader = services.Get<SceneLoader>();

        controller = new MainMenuController(mainMenuView, audioService, sceneLoader);

        Debug.Log("✅ MainMenuEntryPoint initialized successfully!");
    }

    public override void Run()
    {
        Debug.Log("Main Menu Entry Point запущен");
    }

    public override void Cleanup()
    {
        controller = null;
        if (mainMenuView != null)
            Destroy(mainMenuView.gameObject);
    }
}