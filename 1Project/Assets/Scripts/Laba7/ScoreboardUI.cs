using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [Header("UI элементы")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI killsToBossText;
    public TextMeshProUGUI killsToWinText;

    [Header("Настройки отображения")]
    public string killPrefix = "🔪 Убийств: ";
    public string bossPrefix = "👑 До босса: ";
    public string winPrefix = "🏆 До победы: ";

    private void Start()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnKillCountChanged += UpdateScoreboard;
            UpdateScoreboard(GameStats.Instance?.KillCount ?? 0);
        }
        else
        {
            Debug.LogWarning("GameEventManager not found!");
        }
    }

    private void UpdateScoreboard(int currentKills)
    {
        if (killCountText != null)
        {
            killCountText.text = $"{killPrefix}{currentKills}";
        }

        if (killsToBossText != null && GameEventManager.Instance != null)
        {
            int killsToBoss = Mathf.Max(0, GameEventManager.Instance.killToSpawnBoss - currentKills);
            killsToBossText.text = $"{bossPrefix}{killsToBoss}";
        }

        if (killsToWinText != null && GameEventManager.Instance != null)
        {
            int killsToWin = Mathf.Max(0, GameEventManager.Instance.killToWin - currentKills);
            killsToWinText.text = $"{winPrefix}{killsToWin}";
        }
    }

    private void OnDestroy()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnKillCountChanged -= UpdateScoreboard;
        }
    }
}