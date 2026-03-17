using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject enemyPrefab;          // Префаб врага
    public Transform[] spawnPoints;         // Точки спавна
    public float spawnInterval = 2f;        // Интервал между спавнами
    public bool spawnOnStart = true;        // Спавнить ли при старте
    public int maxEnemies = 20;              // Максимум врагов одновременно (0 = без ограничений)

    private int currentEnemyCount = 0;

    private void Start()
    {
        if (spawnOnStart)
        {
            // Запускаем бесконечный спавн
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) // Бесконечный цикл
        {
            // Проверяем лимит врагов
            if (maxEnemies == 0 || currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            // Ждём следующий спавн
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        // Выбираем случайную точку спавна
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Создаём врага
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemyCount++;

        // Подписываемся на смерть
        Health enemyHealth = newEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += OnEnemyDied;
        }
    }

    private void OnEnemyDied()
    {
        currentEnemyCount--;
    }

    // Для отладки: показываем информацию в консоли
    private void Update()
    {
        // Можно добавить отладочный вывод по нажатию клавиши
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"Врагов сейчас: {currentEnemyCount}/{maxEnemies}");
        }
    }
}