using UnityEngine;
using UnityEngine.Audio;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject mainMenuEntryPointPrefab;
    [SerializeField] private GameObject gameplayEntryPointPrefab;
    [SerializeField] private GameObject mainMenuCanvasPrefab; 

    private ServiceLocator services;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeServices();
    }

    private void InitializeServices()
    {
        services = new ServiceLocator();

        var audioService = new AudioService(audioMixer);
        audioService.Initialize();
        services.Register<AudioService>(audioService);

        var saveLoadService = new SaveLoadService();
        saveLoadService.Initialize();
        services.Register<SaveLoadService>(saveLoadService);

        var sceneLoader = new SceneLoader();
        sceneLoader.Initialize();
        services.Register<SceneLoader>(sceneLoader);
    }

    public ServiceLocator GetServices() => services;

    public GameObject GetEntryPointPrefab(string sceneName)
    {
        return sceneName == "MainMenu" ? mainMenuEntryPointPrefab : gameplayEntryPointPrefab;
    }

    public GameObject GetMainMenuCanvasPrefab() => mainMenuCanvasPrefab; // ← ДОБАВИТЬ
}