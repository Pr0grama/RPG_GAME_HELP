using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Основные настройки")]
    public float basePhysicalDamage = 20f;
    public float magicalDamage = 30f;

    [Header("Атака ближнего боя")]
    public Transform attackPoint;
    public float attackRange = 2f;
    public LayerMask enemyLayers;

    [Header("Магическая атака")]
    public GameObject magicProjectilePrefab;
    public Transform magicSpawnPoint;
    public MagicCooldownUI cooldownUI;
    public float magicCooldown = 2f;

    [Header("Система оружия")]
    public WeaponType currentWeaponType = WeaponType.Melee;
    public GameObject meleeWeaponModel;   // Модель для ближнего оружия
    public GameObject rangedWeaponModel;  // Модель для дальнего оружия

    [Header("Бонусы от оружия")]
    public float meleeDamageBonus = 10f;
    public float rangedDamageBonus = 5f;

    private Animator animator;
    private float nextMagicTime = 0f;
    private float currentPhysicalDamage;

    public float CurrentPhysicalDamage => currentPhysicalDamage;
    public WeaponType CurrentWeaponType => currentWeaponType;

    private void Start()
    {
        animator = GetComponent<Animator>();
        UpdateDamage();
        UpdateWeaponModel();

        Debug.Log($"⚔️ PlayerCombat инициализирован. Оружие: {currentWeaponType}, Урон: {currentPhysicalDamage}");
    }

    private void Update()
    {
        // Левая кнопка мыши - физическая атака (зависит от оружия)
        if (Input.GetMouseButtonDown(0))
        {
            PhysicalAttack();
        }

        // Правая кнопка мыши - магическая атака
        if (Input.GetMouseButtonDown(1) && Time.time >= nextMagicTime)
        {
            MagicalAttack();
            nextMagicTime = Time.time + magicCooldown;
            if (cooldownUI != null)
                cooldownUI.StartCooldown();
        }
    }

    /// <summary>
    /// Физическая атака (в зависимости от типа оружия)
    /// </summary>
    void PhysicalAttack()
    {
        if (animator != null)
            animator.SetTrigger("attack");

        Debug.Log($"⚔️ Физическая атака! Оружие: {currentWeaponType}, Урон: {currentPhysicalDamage}");

        if (currentWeaponType == WeaponType.Melee)
        {
            PerformMeleeAttack();
        }
        else
        {
            PerformRangedPhysicalAttack();
        }
    }

    /// <summary>
    /// Ближняя атака
    /// </summary>
    void PerformMeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(currentPhysicalDamage, DamageType.Physical);
                Debug.Log($"🗡️ Нанесён урон ближним боем: {currentPhysicalDamage}");
            }
        }
    }

    /// <summary>
    /// Дальняя физическая атака (стрелы, копья и т.д.)
    /// </summary>
    void PerformRangedPhysicalAttack()
    {
        if (magicProjectilePrefab != null && magicSpawnPoint != null)
        {
            GameObject projectile = Instantiate(magicProjectilePrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);

            // Модифицируем снаряд для физической атаки
            MagicProjectile projScript = projectile.GetComponent<MagicProjectile>();
            if (projScript != null)
            {
                projScript.damage = currentPhysicalDamage;
                projScript.isMagic = false; // Добавьте это поле в MagicProjectile
            }

            Debug.Log($"🏹 Выпущен снаряд дальнего боя! Урон: {currentPhysicalDamage}");
        }
        else
        {
            Debug.LogWarning("❌ magicProjectilePrefab или magicSpawnPoint не назначены для дальней атаки!");
        }
    }

    /// <summary>
    /// Магическая атака
    /// </summary>
    void MagicalAttack()
    {
        Debug.Log($"✨ Магическая атака! Урон: {magicalDamage}");

        if (magicProjectilePrefab != null && magicSpawnPoint != null)
        {
            GameObject projectile = Instantiate(magicProjectilePrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);

            MagicProjectile projScript = projectile.GetComponent<MagicProjectile>();
            if (projScript != null)
            {
                projScript.damage = magicalDamage;
                projScript.isMagic = true;
            }
        }
    }

    // ✅ МЕТОДЫ ДЛЯ СИСТЕМЫ ОРУЖИЯ

    public void EquipMeleeWeapon()
    {
        currentWeaponType = WeaponType.Melee;
        UpdateDamage();
        UpdateWeaponModel();
        Debug.Log($"🗡️ Игрок экипировал БЛИЖНЕЕ оружие! Урон: {currentPhysicalDamage}");
    }

    public void EquipRangedWeapon()
    {
        currentWeaponType = WeaponType.Ranged;
        UpdateDamage();
        UpdateWeaponModel();
        Debug.Log($"🏹 Игрок экипировал ДАЛЬНЕЕ оружие! Урон: {currentPhysicalDamage}");
    }

    private void UpdateDamage()
    {
        if (currentWeaponType == WeaponType.Melee)
        {
            currentPhysicalDamage = basePhysicalDamage + meleeDamageBonus;
        }
        else
        {
            currentPhysicalDamage = basePhysicalDamage + rangedDamageBonus;
        }
    }

    private void UpdateWeaponModel()
    {
        if (meleeWeaponModel != null)
            meleeWeaponModel.SetActive(currentWeaponType == WeaponType.Melee);

        if (rangedWeaponModel != null)
            rangedWeaponModel.SetActive(currentWeaponType == WeaponType.Ranged);
    }

    // Методы для сохранения/загрузки
    public float GetNextMagicTime()
    {
        return nextMagicTime;
    }

    public void SetNextMagicTime(float value)
    {
        nextMagicTime = value;
    }

    public WeaponType GetWeaponType()
    {
        return currentWeaponType;
    }

    public void SetWeaponType(WeaponType type)
    {
        currentWeaponType = type;
        UpdateDamage();
        UpdateWeaponModel();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}