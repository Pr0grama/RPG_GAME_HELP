using UnityEngine;

public class MainMenuEntryPoint : EntryPoint
{
    [SerializeField] private MainMenuView mainMenuView;

    private MainMenuController controller;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        var audioService = services.Get<AudioService>();
        var sceneLoader = services.Get<SceneLoader>();

        controller = new MainMenuController(mainMenuView, audioService, sceneLoader);
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