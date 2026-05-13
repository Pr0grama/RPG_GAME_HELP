using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    // Пустой скрипт-заглушка, чтобы не было ошибок в спавнере
    // Если хочешь полностью избавиться — можешь удалить этот компонент с префабов мобов

    [Header("Система оружия отключена")]
    [Tooltip("Этот скрипт сейчас ничего не делает")]
    public bool weaponSystemDisabled = true;

    private EnemyStateMachine enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        if (enemyAI != null)
        {
            // Просто восстанавливаем нормальные значения на всякий случай
            if (enemyAI.speed < 0.1f) enemyAI.speed = 3.5f;
            if (enemyAI.attackRange < 0.5f) enemyAI.attackRange = 2f;
            if (enemyAI.damage < 1f) enemyAI.damage = 10f;
        }

        Debug.Log($"🛡️ {gameObject.name}: WeaponStats заглушка активна (оружие отключено)");
    }

    // Пустые методы, чтобы не ломать вызовы из EnemySpawner
    public void SetWeapon(int dummy = 0) { }
    public void ApplyWeaponBonuses() { }
}