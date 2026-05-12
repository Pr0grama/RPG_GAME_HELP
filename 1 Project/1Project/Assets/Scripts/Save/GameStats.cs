using UnityEngine;

public class GameStats : MonoBehaviour
{
    private static GameStats instance;
    public static GameStats Instance => instance;

    [SerializeField] private int killCount = 0;
    [SerializeField] private float playTime = 0f;
    [SerializeField] private int currentWave = 0;

    public int KillCount
    {
        get => killCount;
        set
        {
            killCount = value;
            OnStatsChanged?.Invoke();
        }
    }

    public float PlayTime
    {
        get => playTime;
        set
        {
            playTime = value;
            OnStatsChanged?.Invoke();
        }
    }

    public int CurrentWave
    {
        get => currentWave;
        set
        {
            currentWave = value;
            OnStatsChanged?.Invoke();
        }
    }

    public System.Action OnStatsChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playTime += Time.deltaTime;
        }
    }

    public void AddKill()
    {
        KillCount++;
    }

    public void ResetStats()
    {
        killCount = 0;
        playTime = 0f;
        currentWave = 0;
        OnStatsChanged?.Invoke();
    }
}