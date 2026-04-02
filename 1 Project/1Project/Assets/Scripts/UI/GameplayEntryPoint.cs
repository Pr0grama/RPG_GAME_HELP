using UnityEngine;

public class GameplayEntryPoint : EntryPoint
{
    [SerializeField] private GameMenuView gameMenuViewPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private GameMenuView gameMenuView;
    private GameMenuController menuController;
    private GameObject playerInstance;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        if (gameMenuViewPrefab != null)
        {
            var canvasInstance = Instantiate(gameMenuViewPrefab);
            gameMenuView = canvasInstance.GetComponent<GameMenuView>();
            // ⚠️ УДАЛИТЬ DontDestroyOnLoad - пусть Canvas живет только в этой сцене
            // DontDestroyOnLoad(canvasInstance); // ← ЗАКОММЕНТИРОВАТЬ ИЛИ УДАЛИТЬ
            Debug.Log("✅ GameMenuCanvas created from prefab");
        }
        else
        {
            Debug.LogError("❌ gameMenuViewPrefab is NULL!");
            return;
        }

        var saveLoad = services.Get<SaveLoadService>();
        var sceneLoader = services.Get<SceneLoader>();

        menuController = new GameMenuController(gameMenuView, saveLoad, sceneLoader);

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

        if (gameMenuView != null)
            Destroy(gameMenuView.gameObject);

        Debug.Log("✅ GameplayEntryPoint cleaned up");
    }
}