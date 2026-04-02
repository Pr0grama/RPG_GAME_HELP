using System;
using UnityEngine;

namespace UI.Models
{
    // Модель для главного меню
    [Serializable]
    public class MainMenuModel
    {
        public bool isSettingsOpen = false;
        public float musicVolume = 0.75f;
        public bool isQuitDialogOpen = false;
    }

    // Модель для игрового меню
    [Serializable]
    public class GameMenuModel
    {
        public bool isMenuOpen = false;
        public bool hasSaveFile = false;
        public float lastSaveTime = 0f;
    }

    // Модель для настроек
    [Serializable]
    public class SettingsModel
    {
        public float masterVolume = 0.8f;
        public float musicVolume = 0.75f;
        public float sfxVolume = 0.7f;
        public bool isFullscreen = true;
        public int qualityLevel = 2; // 0=Low, 1=Medium, 2=High
        public float mouseSensitivity = 100f;

        // Конструктор по умолчанию
        public SettingsModel()
        {
            masterVolume = 0.8f;
            musicVolume = 0.75f;
            sfxVolume = 0.7f;
            isFullscreen = true;
            qualityLevel = 2;
            mouseSensitivity = 100f;
        }

        // Загрузка сохраненных настроек
        public void LoadFromPlayerPrefs()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
            isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
        }

        // Сохранение настроек
        public void SaveToPlayerPrefs()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.Save();
        }
    }

    // Модель для данных сохранения игры
    [Serializable]
    public class SaveGameModel
    {
        public string saveName;
        public DateTime saveDate;
        public int playerHealth;
        public int playerMaxHealth;
        public Vector3 playerPosition;
        public int killCount;
        public float playTime;
        public int currentWave;

        public SaveGameModel()
        {
            saveName = "";
            saveDate = DateTime.Now;
            playerHealth = 100;
            playerMaxHealth = 100;
            playerPosition = Vector3.zero;
            killCount = 0;
            playTime = 0f;
            currentWave = 0;
        }

        // Конвертация в JSON для сохранения
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        // Загрузка из JSON
        public static SaveGameModel FromJson(string json)
        {
            return JsonUtility.FromJson<SaveGameModel>(json);
        }
    }

    // Модель для уведомлений/сообщений
    [Serializable]
    public class NotificationModel
    {
        public string message;
        public float displayTime;
        public bool isActive;

        public NotificationModel()
        {
            message = "";
            displayTime = 2f;
            isActive = false;
        }

        public void ShowMessage(string msg, float time = 2f)
        {
            message = msg;
            displayTime = time;
            isActive = true;
        }

        public void Hide()
        {
            isActive = false;
            message = "";
        }
    }

    // Модель для диалогового окна
    [Serializable]
    public class DialogModel
    {
        public string title;
        public string message;
        public string confirmText;
        public string cancelText;
        public Action onConfirm;
        public Action onCancel;
        public bool isOpen;

        public DialogModel()
        {
            title = "Подтверждение";
            message = "";
            confirmText = "Да";
            cancelText = "Нет";
            isOpen = false;
        }

        public void Show(string title, string message, Action onConfirm, Action onCancel = null,
            string confirmText = "Да", string cancelText = "Нет")
        {
            this.title = title;
            this.message = message;
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            this.confirmText = confirmText;
            this.cancelText = cancelText;
            isOpen = true;
        }

        public void Hide()
        {
            isOpen = false;
            onConfirm = null;
            onCancel = null;
        }
    }
}