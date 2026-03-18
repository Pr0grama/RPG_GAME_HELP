using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float physicalDamage = 20f;
    public float magicalDamage = 30f;
    public Transform attackPoint;
    public float attackRange = 2f;
    public LayerMask enemyLayers;
    public GameObject magicProjectilePrefab; // префаб снаряда
    public Transform magicSpawnPoint;        // откуда вылетает магия
    public MagicCooldownUI cooldownUI;

    [Header("Magic Cooldown")]
    public float magicCooldown = 2f;         // длительность перезарядки в секундах
    private float nextMagicTime = 0f;        // когда можно будет снова кастовать

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Физическая атака (левая кнопка мыши)
        if (Input.GetMouseButtonDown(0))
        {
            PhysicalAttack();
        }

        // Магическая атака (правая кнопка мыши) с проверкой кулдауна
        if (Input.GetMouseButtonDown(1) && Time.time >= nextMagicTime)
        {
            MagicalAttack();
            nextMagicTime = Time.time + magicCooldown; // устанавливаем время следующей возможной атаки

            if (cooldownUI != null)
                cooldownUI.StartCooldown(); // ← запуск визуала
        }
    }

    void PhysicalAttack()
    {
        // Запускаем анимацию
        animator.SetTrigger("attack");

        // Находим врагов в зоне поражения
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Наносим урон каждому врагу
        foreach (Collider enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(physicalDamage, DamageType.Physical);
                Debug.Log("Нанесён физический урон: " + physicalDamage);
            }
        }
    }

    void MagicalAttack()
    {
        Debug.Log("✨ Магическая атака!");

        if (magicProjectilePrefab != null && magicSpawnPoint != null)
        {
            // Создаём снаряд
            GameObject projectile = Instantiate(magicProjectilePrefab,
                                               magicSpawnPoint.position,
                                               magicSpawnPoint.rotation);
        }
    }

    // Для визуализации зоны атаки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}