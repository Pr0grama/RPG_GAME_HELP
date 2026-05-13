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
    public bool spawnRandomType = true;

    [Header("Настройки оружия")]
    public bool randomWeapon = true;
    [Range(0, 1)] public float swordChance = 0.5f;
    [Range(0, 1)] public float staffChance = 0.5f;

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

        // ✅ НАСТРАИВАЕМ ТИП ВРАГА через isRanged (убираем enemyType, его нет в EnemyStateMachine)
        EnemyStateMachine enemyAI = newEnemy.GetComponent<EnemyStateMachine>();
        if (enemyAI != null)
        {
            // Определяем тип по selectedType.enemyType
            if (selectedType.enemyType == EnemyType.Ranged)
            {
                enemyAI.isRanged = true;
            }
            else
            {
                enemyAI.isRanged = false;
            }
        }

        // ✅ ДОБАВЛЯЕМ РАНДОМНОЕ ОРУЖИЕ
        if (randomWeapon)
        {
            WeaponStats weaponStats = newEnemy.GetComponent<WeaponStats>();
            if (weaponStats != null)
            {
                WeaponBuffType randomWeaponType = GetRandomWeaponType();
                weaponStats.SetWeapon(randomWeaponType);
                Debug.Log($"🗡️ Спавнен {(selectedType.enemyType == EnemyType.Ranged ? "Дальний" : "Ближний")} с оружием: {randomWeaponType}");
            }
            else
            {
                Debug.LogWarning($"{newEnemy.name}: Нет компонента WeaponStats!");
            }
        }

        // Подписываемся на смерть
        Health enemyHealth = newEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += OnEnemyDied;
        }

        Debug.Log($"Спавнен враг типа: {selectedType.enemyType}");
    }

    // ✅ ВЫБОР СЛУЧАЙНОГО ОРУЖИЯ
    private WeaponBuffType GetRandomWeaponType()
    {
        float random = Random.Range(0f, 1f);

        // Нормализуем шансы (если сумма не равна 1)
        float total = swordChance + staffChance;
        float normalizedSwordChance = swordChance / total;

        if (random < normalizedSwordChance)
        {
            return WeaponBuffType.Sword;
        }
        else
        {
            return WeaponBuffType.Staff;
        }
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
            GameObject newEnemy = Instantiate(enemySettings.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            currentEnemyCount++;

            // Настраиваем тип врага
            EnemyStateMachine enemyAI = newEnemy.GetComponent<EnemyStateMachine>();
            if (enemyAI != null)
            {
                enemyAI.isRanged = (type == EnemyType.Ranged);
            }

            // Добавляем рандомное оружие
            if (randomWeapon)
            {
                WeaponStats weaponStats = newEnemy.GetComponent<WeaponStats>();
                if (weaponStats != null)
                {
                    WeaponBuffType randomWeaponType = GetRandomWeaponType();
                    weaponStats.SetWeapon(randomWeaponType);
                }
            }
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