using UnityEngine;

public enum WeaponBuffType
{
    None,
    Sword,   // Меч
    Staff    // Посох
}

public class WeaponStats : MonoBehaviour
{
    [Header("Текущее оружие")]
    public WeaponBuffType currentWeapon = WeaponBuffType.None;

    [Header("Бонусы для меча")]
    public float swordSpeedMultiplier = 1.3f;
    public float swordRangeMultiplier = 0.7f;
    public float swordDamageMultiplier = 1f;

    [Header("Бонусы для посоха")]
    public float staffSpeedMultiplier = 0.7f;
    public float staffRangeMultiplier = 1f;
    public float staffDamageMultiplier = 1.5f;

    [Header("Визуальный индикатор")]
    public GameObject swordIcon;
    public GameObject staffIcon;
    public float iconHeight = 2.5f;

    private GameObject currentIcon;
    private EnemyStateMachine enemyAI;
    private EnemyType enemyType;

    private float originalSpeed;
    private float originalAttackRange;
    private float originalDamage;
    private float originalRangedAttackRange;

    private void Start()
    {
        enemyAI = GetComponent<EnemyStateMachine>();
        if (enemyAI == null) return;

        originalSpeed = enemyAI.speed;
        originalAttackRange = enemyAI.attackRange;
        originalDamage = enemyAI.damage;
        originalRangedAttackRange = enemyAI.rangedAttackRange;

        enemyType = enemyAI.isRanged ? EnemyType.Ranged : EnemyType.Melee;

        ApplyWeaponBonuses();
        CreateWeaponIcon();

        Debug.Log($"⚔️ {gameObject.name}: экипирован {currentWeapon}, Тип: {enemyType}");
    }

    // ✅ ПУБЛИЧНЫЙ МЕТОД ДЛЯ ПРИМЕНЕНИЯ БОНУСОВ (вызывается из спавнера)
    public void ApplyWeaponBonuses()
    {
        if (enemyAI == null) enemyAI = GetComponent<EnemyStateMachine>();
        if (enemyAI == null) return;

        // Сбрасываем статы до исходных
        enemyAI.speed = originalSpeed;
        enemyAI.attackRange = originalAttackRange;
        enemyAI.damage = originalDamage;
        enemyAI.rangedAttackRange = originalRangedAttackRange;

        if (currentWeapon == WeaponBuffType.None) return;

        if (enemyType == EnemyType.Melee)
        {
            if (currentWeapon == WeaponBuffType.Sword)
            {
                Debug.Log($"🗡️ {gameObject.name} (Ближний) с мечом: статы без изменений");
            }
            else if (currentWeapon == WeaponBuffType.Staff)
            {
                enemyAI.speed *= staffSpeedMultiplier;
                enemyAI.damage *= staffDamageMultiplier;
                Debug.Log($"🪄 {gameObject.name} (Ближний) с посохом: скорость {enemyAI.speed:F1}, урон {enemyAI.damage:F1}");
            }
        }
        else if (enemyType == EnemyType.Ranged)
        {
            if (currentWeapon == WeaponBuffType.Sword)
            {
                enemyAI.speed *= swordSpeedMultiplier;
                enemyAI.rangedAttackRange *= swordRangeMultiplier;
                Debug.Log($"🗡️ {gameObject.name} (Дальний) с мечом: скорость {enemyAI.speed:F1}, дальность {enemyAI.rangedAttackRange:F1}");
            }
            else if (currentWeapon == WeaponBuffType.Staff)
            {
                Debug.Log($"🪄 {gameObject.name} (Дальний) с посохом: статы без изменений");
            }
        }
    }

    // ✅ МЕТОД ДЛЯ УСТАНОВКИ ОРУЖИЯ ИЗ СПАВНЕРА
    public void SetWeapon(WeaponBuffType weapon)
    {
        currentWeapon = weapon;

        // Обновляем иконку
        if (currentIcon != null) Destroy(currentIcon);
        CreateWeaponIcon();

        // Применяем бонусы
        ApplyWeaponBonuses();

        Debug.Log($"⚔️ {gameObject.name}: экипирован {currentWeapon}");
    }

    private void CreateWeaponIcon()
    {
        if (currentIcon != null) Destroy(currentIcon);

        GameObject iconPrefab = null;

        switch (currentWeapon)
        {
            case WeaponBuffType.Sword:
                iconPrefab = swordIcon;
                break;
            case WeaponBuffType.Staff:
                iconPrefab = staffIcon;
                break;
            default:
                return;
        }

        if (iconPrefab != null)
        {
            currentIcon = Instantiate(iconPrefab, transform);
            currentIcon.transform.localPosition = new Vector3(0, iconHeight, 0);
            currentIcon.transform.localRotation = Quaternion.identity;
            currentIcon.AddComponent<Billboard>();
        }
    }

    private void OnDestroy()
    {
        if (currentIcon != null) Destroy(currentIcon);
    }
}