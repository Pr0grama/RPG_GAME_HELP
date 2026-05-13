using UnityEngine;
using System.Collections.Generic;

public class WeaponSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSet
    {
        public string weaponName;
        public GameObject weaponPrefab;
        public WeaponType weaponType;
        public float spawnWeight = 1f;
    }

    [Header("Настройки спавна")]
    public List<WeaponSet> availableWeapons;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;
    public bool spawnOnStart = true;

    private List<GameObject> spawnedWeapons = new List<GameObject>();

    private void Start()
    {
        if (spawnOnStart)
        {
            InvokeRepeating(nameof(SpawnRandomWeapon), 1f, spawnInterval);
        }
    }

    private void SpawnRandomWeapon()
    {
        if (spawnPoints.Length == 0 || availableWeapons.Count == 0) return;

        // Выбираем случайное оружие
        WeaponSet selected = GetRandomWeapon();
        if (selected == null) return;

        // Выбираем случайную точку спавна
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Создаём оружие
        GameObject weapon = Instantiate(selected.weaponPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedWeapons.Add(weapon);

        // Добавляем компонент для подбора
        PickupWeapon pickup = weapon.GetComponent<PickupWeapon>();
        if (pickup == null)
            pickup = weapon.AddComponent<PickupWeapon>();

        pickup.Initialize(selected.weaponType);

        Debug.Log($"🗡️ Спавнен предмет: {selected.weaponName} в точке {spawnPoint.position}");

        // Очистка старых предметов
        CleanupOldWeapons();
    }

    private WeaponSet GetRandomWeapon()
    {
        float totalWeight = 0f;
        foreach (var weapon in availableWeapons)
            totalWeight += weapon.spawnWeight;

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var weapon in availableWeapons)
        {
            currentWeight += weapon.spawnWeight;
            if (randomWeight <= currentWeight)
                return weapon;
        }

        return availableWeapons[0];
    }

    private void CleanupOldWeapons()
    {
        spawnedWeapons.RemoveAll(w => w == null);

        // Удаляем самые старые, если их слишком много
        while (spawnedWeapons.Count > 10)
        {
            if (spawnedWeapons[0] != null)
                Destroy(spawnedWeapons[0]);
            spawnedWeapons.RemoveAt(0);
        }
    }
}