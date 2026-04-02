using UnityEngine;

public class GameplayEntryPoint : EntryPoint
{
    [SerializeField] private GameMenuView gameMenuView;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private GameMenuController menuController;
    private GameObject playerInstance;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        var saveLoad = services.Get<SaveLoadService>();
        var sceneLoader = services.Get<SceneLoader>();

        menuController = new GameMenuController(gameMenuView, saveLoad, sceneLoader);

        // Спавним игрока
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }
    }

    public override void Run()
    {
        Debug.Log("Gameplay Entry Point запущен");
    }

    private void Update()
    {
        menuController?.Update();
    }

    public override void Cleanup()
    {
        menuController = null;
        if (playerInstance != null)
            Destroy(playerInstance);
    }
}