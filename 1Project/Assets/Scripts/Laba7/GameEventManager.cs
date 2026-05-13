using UnityEngine;
using System;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    [Header("Настройки")]
    public int killToSpawnBoss = 3;
    public int killToWin = 5;

    [Header("Префабы")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    [Header("Звуки")]
    public AudioClip victorySound;

    private int killCount = 0;
    private bool bossSpawned = false;
    private bool victoryPlayed = false;
    private AudioSource audioSource;

    public event Action<int> OnKillCountChanged;
    public event Action OnBossSpawned;
    public event Action OnVictory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        // Подписываемся на события смерти врагов
        if (GameStats.Instance != null)
        {
            GameStats.Instance.OnStatsChanged += CheckKillCount;
            killCount = GameStats.Instance.KillCount;
            CheckKillCount();
        }
    }

    private void CheckKillCount()
    {
        if (GameStats.Instance == null) return;

        int currentKills = GameStats.Instance.KillCount;

        // Вызываем событие для UI
        OnKillCountChanged?.Invoke(currentKills);

        // Проверяем спавн босса
        if (!bossSpawned && currentKills >= killToSpawnBoss)
        {
            SpawnBoss();
        }

        // Проверяем победу
        if (!victoryPlayed && currentKills >= killToWin)
        {
            Victory();
        }
    }

    private void SpawnBoss()
    {
        if (bossSpawned) return;
        if (bossPrefab == null)
        {
            Debug.LogError("❌ Boss prefab not assigned in GameEventManager!");
            return;
        }

        bossSpawned = true;

        Transform spawnPoint = bossSpawnPoint != null ? bossSpawnPoint : transform;
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"👑 БОСС появился после {killCount} убийств!");
        OnBossSpawned?.Invoke();

        // Можно добавить звук появления босса
        // AudioSource.PlayClipAtPoint(bossSpawnSound, spawnPoint.position);
    }

    private void Victory()
    {
        if (victoryPlayed) return;
        victoryPlayed = true;

        Debug.Log($"🏆 ПОБЕДА! Убито {killCount} врагов!");

        // Проигрываем победную мелодию
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        OnVictory?.Invoke();

        // Можно показать панель победы
        // UIManager.Instance.ShowVictoryPanel();
    }

    private void OnDestroy()
    {
        if (GameStats.Instance != null)
        {
            GameStats.Instance.OnStatsChanged -= CheckKillCount;
        }
    }
}