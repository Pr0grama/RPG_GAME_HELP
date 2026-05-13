using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    private void Awake()
    {
        // Проверяем, есть ли уже экземпляр музыкального плеера
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Если уже есть, уничтожаем этот
            Destroy(gameObject);
        }
    }
}