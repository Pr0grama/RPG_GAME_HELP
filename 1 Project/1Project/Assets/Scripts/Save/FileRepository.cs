using UnityEngine;
using System.IO;

public class FileRepository : IPlayerRepository
{
    private string savePath;
    private const string SAVE_FILE_NAME = "player_save.json";

    public FileRepository()
    {
        // Получаем путь к папке проекта (где лежит Assets)
        string projectPath = Application.dataPath; // .../ProjectName/Assets
        projectPath = Directory.GetParent(projectPath).FullName; // .../ProjectName

        // Создаём папку Saves, если её нет
        string savesFolder = Path.Combine(projectPath, "Saves");
        if (!Directory.Exists(savesFolder))
        {
            Directory.CreateDirectory(savesFolder);
            Debug.Log($"?? Создана папка: {savesFolder}");
        }

        // Полный путь к файлу сохранения
        savePath = Path.Combine(savesFolder, SAVE_FILE_NAME);
        Debug.Log($"?? Файл сохранения будет здесь: {savePath}");
    }

    public void Save(PlayerModel model)
    {
        try
        {
            string json = JsonUtility.ToJson(model, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"? Игра сохранена в проект: {savePath}");
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
            string json = File.ReadAllText(savePath);
            Debug.Log($"?? Загрузка из файла проекта: {savePath}");
            return JsonUtility.FromJson<PlayerModel>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"? Ошибка загрузки: {e.Message}");
            return null;
        }
    }

    public bool HasSave() => File.Exists(savePath);

    public void Delete()
    {
        if (HasSave())
        {
            File.Delete(savePath);
            Debug.Log($"??? Файл сохранения удалён: {savePath}");
        }
    }

    public string GetSaveFilePath() => savePath;
}