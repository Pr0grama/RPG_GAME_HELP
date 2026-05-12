using UnityEngine;

public class PlayerPrefsRepository : IPlayerRepository
{
    private const string SAVE_KEY = "PlayerSaveData";
    private const string SAVE_EXISTS_KEY = "PlayerSaveExists";

    public void Save(PlayerModel model)
    {
        try
        {
            string json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.SetInt(SAVE_EXISTS_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log($"? Игра сохранена через PlayerPrefs. Размер JSON: {json.Length} байт");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"? Ошибка сохранения: {e.Message}");
        }
    }

    public PlayerModel Load()
    {
        try
        {
            if (!HasSave()) return null;

            string json = PlayerPrefs.GetString(SAVE_KEY);
            PlayerModel model = JsonUtility.FromJson<PlayerModel>(json);
            Debug.Log($"? Загрузка игры через PlayerPrefs завершена");
            return model;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"? Ошибка загрузки: {e.Message}");
            return null;
        }
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_EXISTS_KEY) && PlayerPrefs.GetInt(SAVE_EXISTS_KEY) == 1;
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(SAVE_EXISTS_KEY);
        PlayerPrefs.Save();
        Debug.Log("??? Сохранение удалено");
    }

    public string GetSaveFilePath()
    {
        return Application.persistentDataPath + "/player_save.prefs";
    }
}