using UnityEngine;
using System;

public class PlayerInteractor : IPlayerInteractor
{
    private IPlayerRepository repository;
    private PlayerModel currentModel;

    public event Action<PlayerModel> OnGameSaved;
    public event Action<PlayerModel> OnGameLoaded;

    public PlayerInteractor(IPlayerRepository repo)
    {
        repository = repo;
        currentModel = new PlayerModel();
        if (repository.HasSave())
        {
            var loaded = repository.Load();
            if (loaded != null) currentModel.CopyFrom(loaded);
        }
    }

    public void SaveCurrentState()
    {
        try
        {
            CollectCurrentGameState();
            repository.Save(currentModel);
            OnGameSaved?.Invoke(currentModel);
            Debug.Log($"?? СОХРАНЕНО: HP={currentModel.health}, MagicCD={currentModel.nextMagicTime}");
        }
        catch (Exception e) { Debug.LogError($"? {e.Message}"); }
    }

    public void LoadLastState()
    {
        try
        {
            if (!HasSave()) { Debug.LogWarning("?? Нет сохранений!"); return; }
            var loaded = repository.Load();
            if (loaded != null)
            {
                currentModel.CopyFrom(loaded);
                ApplyGameStateToObjects();
                OnGameLoaded?.Invoke(currentModel);
                Debug.Log($"?? ЗАГРУЖЕНО: HP={currentModel.health}, MagicCD={currentModel.nextMagicTime}");
            }
        }
        catch (Exception e) { Debug.LogError($"? {e.Message}"); }
    }

    public bool HasSave() => repository.HasSave();
    public PlayerModel GetCurrentModel() => currentModel;
    public void DeleteSave() { repository.Delete(); currentModel = new PlayerModel(); }

    private void CollectCurrentGameState()
    {
        // Прямой поиск модели по имени
        GameObject playerModel = GameObject.Find("Paladin WProp J Nordstrom");
        if (playerModel == null)
        {
            Debug.LogError("? Paladin WProp J Nordstrom не найден!");
            return;
        }

        Debug.Log($"?? Найдена модель: {playerModel.name}");

        Health health = playerModel.GetComponent<Health>();
        if (health != null)
        {
            currentModel.health = health.currentHealth;
            Debug.Log($"?? Сохранено HP: {currentModel.health}");
        }

        Mana mana = playerModel.GetComponent<Mana>();
        if (mana != null) currentModel.mana = mana.currentMana;

        PlayerCombat combat = playerModel.GetComponent<PlayerCombat>();
        if (combat != null) currentModel.nextMagicTime = combat.GetNextMagicTime();

        // Сохраняем позицию корня
        GameObject playerRoot = GameObject.FindGameObjectWithTag("Player");
        if (playerRoot != null) currentModel.position = playerRoot.transform.position;
    }

    private void ApplyGameStateToObjects()
    {
        // Прямой поиск модели по имени
        GameObject playerModel = GameObject.Find("Paladin WProp J Nordstrom");
        if (playerModel == null)
        {
            Debug.LogError("? Paladin WProp J Nordstrom не найден!");
            return;
        }

        Debug.Log($"?? Найдена модель для восстановления: {playerModel.name}");

        Health health = playerModel.GetComponent<Health>();
        if (health != null)
        {
            Debug.Log($"?? БЫЛО HP: {health.currentHealth}");
            health.currentHealth = currentModel.health;
            Debug.Log($"?? СТАЛО HP: {health.currentHealth}");
        }

        Mana mana = playerModel.GetComponent<Mana>();
        if (mana != null) mana.currentMana = currentModel.mana;

        PlayerCombat combat = playerModel.GetComponent<PlayerCombat>();
        if (combat != null) combat.SetNextMagicTime(currentModel.nextMagicTime);

        // Восстанавливаем позицию корня
        GameObject playerRoot = GameObject.FindGameObjectWithTag("Player");
        if (playerRoot != null) playerRoot.transform.position = currentModel.position;

        Debug.Log($"? Загрузка завершена. HP={currentModel.health}");
    }
}