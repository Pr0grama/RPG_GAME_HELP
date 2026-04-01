using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTypeSettings
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
        public int spawnCount = 5;
        public float spawnWeight = 1f; // Вес для случайного спавна
    }

    [Header("Настройки спавна")]
    public List<EnemyTypeSettings> enemyTypes = new List<EnemyTypeSettings>();
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    public bool spawnOnStart = true;
    public int maxEnemies = 20;
    public bool spawnRandomType = true; // Если true - спавнит случайный тип, если false - по очереди

    private int currentEnemyCount = 0;
    private int currentTypeIndex = 0;

    private void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (maxEnemies == 0 || currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyTypes.Count == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Нет настроенных типов врагов или точек спавна!");
            return;
        }

        // Выбираем тип врага
        EnemyTypeSettings selectedType = null;

        if (spawnRandomType)
        {
            selectedType = GetRandomEnemyType();
        }
        else
        {
            selectedType = enemyTypes[currentTypeIndex];
            currentTypeIndex = (currentTypeIndex + 1) % enemyTypes.Count;
        }

        if (selectedType == null || selectedType.enemyPrefab == null)
        {
            Debug.LogWarning($"Префаб для типа {selectedType?.enemyType} не назначен!");
            return;
        }

        // Выбираем случайную точку спавна
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Создаем врага
        GameObject newEnemy = Instantiate(selectedType.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemyCount++;

        // Настраиваем тип врага
        EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.enemyType = selectedType.enemyType;
        }

        // Подписываемся на смерть
        Health enemyHealth = newEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += OnEnemyDied;
        }

        Debug.Log($"Спавнен враг типа: {selectedType.enemyType}");
    }

    private EnemyTypeSettings GetRandomEnemyType()
    {
        // Вычисляем общий вес
        float totalWeight = 0f;
        foreach (var type in enemyTypes)
        {
            totalWeight += type.spawnWeight;
        }

        // Выбираем случайный вес
        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var type in enemyTypes)
        {
            currentWeight += type.spawnWeight;
            if (randomWeight <= currentWeight)
            {
                return type;
            }
        }

        return enemyTypes[0];
    }

    private void OnEnemyDied()
    {
        currentEnemyCount--;
    }

    // Метод для ручного спавна конкретного типа
    public void SpawnSpecificEnemy(EnemyType type)
    {
        var enemySettings = enemyTypes.Find(e => e.enemyType == type);
        if (enemySettings != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemySettings.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            currentEnemyCount++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"Врагов сейчас: {currentEnemyCount}/{maxEnemies}");
            foreach (var type in enemyTypes)
            {
                Debug.Log($"- {type.enemyType}: префаб {(type.enemyPrefab != null ? "назначен" : "НЕ назначен")}");
            }
        }
    }
}