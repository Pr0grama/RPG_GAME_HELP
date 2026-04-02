using UnityEngine;

public class MainMenuController
{
    private MainMenuView view;
    private AudioService audioService;
    private SceneLoader sceneLoader;

    public MainMenuController(MainMenuView view, AudioService audio, SceneLoader loader)
    {
        this.view = view;
        this.audioService = audio;
        this.sceneLoader = loader;

        SetupButtons();
        SetupVolumeSlider();
    }

    private void SetupButtons()
    {
        view.PlayButton.onClick.AddListener(OnPlayClicked);
        view.SettingsButton.onClick.AddListener(OnSettingsClicked);
        view.CloseSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
    }

    private void SetupVolumeSlider()
    {
        view.VolumeSlider.value = audioService.GetMusicVolume();
        view.VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnPlayClicked()
    {
        sceneLoader.LoadScene("GameScene");
    }

    private void OnSettingsClicked()
    {
        view.SettingsPanel.SetActive(true);
    }

    private void OnCloseSettingsClicked()
    {
        view.SettingsPanel.SetActive(false);
    }

    private void OnVolumeChanged(float value)
    {
        audioService.SetMusicVolume(value);
    }
}

public class GameMenuController
{
    private GameMenuView view;
    private SaveLoadService saveLoad;
    private SceneLoader sceneLoader;
    private bool isMenuOpen = false;

    public GameMenuController(GameMenuView view, SaveLoadService save, SceneLoader loader)
    {
        this.view = view;
        this.saveLoad = save;
        this.sceneLoader = loader;

        SetupButtons();
        view.MenuPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        view.MainMenuButton.onClick.AddListener(OnMainMenuClicked);
        view.SaveButton.onClick.AddListener(OnSaveClicked);
        view.LoadButton.onClick.AddListener(OnLoadClicked);
        view.ResumeButton.onClick.AddListener(ToggleMenu);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        view.MenuPanel.SetActive(isMenuOpen);
        Time.timeScale = isMenuOpen ? 0f : 1f;
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;

        // Находим и уничтожаем GameplayEntryPoint перед переходом
        var entryPoint = Object.FindObjectOfType<GameplayEntryPoint>();
        if (entryPoint != null)
        {
            entryPoint.Cleanup();
            Object.Destroy(entryPoint.gameObject);
        }

        // Также уничтожаем сам Canvas, если он еще существует
        if (view != null)
        {
            Object.Destroy(view.gameObject);
        }

        sceneLoader.LoadScene("MainMenu");
    }

    private void OnSaveClicked()
    {
        var gameData = new SaveLoadService.GameData();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health health = player.GetComponent<Health>();
            if (health != null)
                gameData.playerHealth = (int)health.currentHealth;
            gameData.playerPosition = player.transform.position;
        }
        saveLoad.SaveGame(gameData);
        Debug.Log("Игра сохранена!");
    }

    private void OnLoadClicked()
    {
        var gameData = saveLoad.LoadGame();
        if (gameData != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health health = player.GetComponent<Health>();
                if (health != null)
                    health.currentHealth = gameData.playerHealth;
                player.transform.position = gameData.playerPosition;
            }
            Debug.Log("Игра загружена!");
        }
        else
        {
            Debug.Log("Нет сохранений!");
        }
    }
}