using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadService : IService
{
    private string savePath;
    private const string SaveFileName = "game_save.dat";

    [System.Serializable]
    public class GameData
    {
        public int playerHealth;
        public Vector3 playerPosition;
        public int killCount;
        public float playTime;
    }

    public SaveLoadService()
    {
        savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    public void Initialize() { }

    public void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Игра сохранена: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                GameData data = JsonUtility.FromJson<GameData>(json);
                Debug.Log($"Загрузка игры: {savePath}");
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
        }
        return null;
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void Cleanup() { }
}