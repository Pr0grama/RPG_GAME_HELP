using UnityEngine;

public class MainMenuEntryPoint : EntryPoint
{
    [SerializeField] private MainMenuView mainMenuView;

    private MainMenuController controller;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        // ПРОВЕРКА: mainMenuView назначен?
        if (mainMenuView == null)
        {
            Debug.LogError("❌ MainMenuEntryPoint: mainMenuView is NULL! Assign it in the inspector!");
            return;
        }

        var audioService = services.Get<AudioService>();
        var sceneLoader = services.Get<SceneLoader>();

        // ПРОВЕРКА: сервисы получены?
        if (audioService == null) Debug.LogError("❌ AudioService is NULL!");
        if (sceneLoader == null) Debug.LogError("❌ SceneLoader is NULL!");

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
    }
}