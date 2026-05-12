using UnityEngine;

public class GameplayEntryPoint : EntryPoint
{
    [SerializeField] private GameMenuView gameMenuViewPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private bool useFileRepository = true; // true - файл, false - PlayerPrefs

    private GameMenuView gameMenuView;
    private GameMenuController menuController;
    private GameObject playerInstance;
    private IPlayerInteractor playerInteractor;

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);

        // 1. Создаём репозиторий
        IPlayerRepository repository;
        if (useFileRepository)
        {
            repository = new FileRepository();
            Debug.Log("📁 Используется файловое хранилище");
        }
        else
        {
            repository = new PlayerPrefsRepository();
            Debug.Log("💾 Используется PlayerPrefs хранилище");
        }

        // 2. Создаём интерактор
        playerInteractor = new PlayerInteractor(repository);

        // 3. Регистрируем в ServiceLocator для доступа из других мест
        serviceLocator.Register<IPlayerInteractor>(playerInteractor);

        // 4. Создаём UI
        if (gameMenuViewPrefab != null)
        {
            var canvasInstance = Instantiate(gameMenuViewPrefab);
            gameMenuView = canvasInstance.GetComponent<GameMenuView>();
            Debug.Log("✅ GameMenuCanvas создан из префаба");
        }
        else
        {
            Debug.LogError("❌ gameMenuViewPrefab is NULL!");
            return;
        }

        // 5. Создаём контроллер
        var sceneLoader = services.Get<SceneLoader>();
        menuController = new GameMenuController(gameMenuView, playerInteractor, sceneLoader);

        // 6. Создаём игрока
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

            // Если есть сохранение, применяем его
            if (playerInteractor.HasSave())
            {
                Debug.Log("📀 Найдено сохранение, загружаем...");
                playerInteractor.LoadLastState();
            }
        }

        // 7. Подписываемся на события для отладки
        playerInteractor.OnGameSaved += OnGameSaved;
        playerInteractor.OnGameLoaded += OnGameLoaded;
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

        // Отписываемся от событий
        if (playerInteractor != null)
        {
            playerInteractor.OnGameSaved -= OnGameSaved;
            playerInteractor.OnGameLoaded -= OnGameLoaded;
        }

        Debug.Log("✅ GameplayEntryPoint очищен");
    }

    private void OnGameSaved(PlayerModel model)
    {
        Debug.Log($"🎉 Событие сохранения: HP={model.health}, Kills={model.killCount}");
    }

    private void OnGameLoaded(PlayerModel model)
    {
        Debug.Log($"🎉 Событие загрузки: HP={model.health}, Kills={model.killCount}");
    }
}