using UnityEngine;

public enum EnemyType
{
    Melee,   // Ближний бой
    Ranged   // Дальний бой
}

public class EnemyAI : MonoBehaviour
{
    [Header("Основные настройки")]
    public EnemyType enemyType = EnemyType.Melee;
    public float speed = 3f;
    public Transform player;

    [Header("Настройки ближнего боя")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    [Header("Настройки дальнего боя")]
    public float rangedAttackRange = 10f;
    public float rangedAttackCooldown = 2f;
    public float rangedDamage = 15f;
    public GameObject projectilePrefab;      // Префаб снаряда
    public Transform shootPoint;              // Точка вылета снаряда
    public float projectileSpeed = 15f;

    [Header("Настройки движения для дальнего боя")]
    public float minDistanceToPlayer = 5f;    // Минимальная дистанция до игрока
    public float maxDistanceToPlayer = 8f;    // Максимальная дистанция до игрока

    private float nextAttackTime = 0f;
    private Health health;
    private Animator animator;
    private bool isAttacking = false;

    private void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        // Находим игрока
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Если нет точки стрельбы, используем позицию врага
        if (shootPoint == null && enemyType == EnemyType.Ranged)
        {
            shootPoint = transform;
        }

        // Подписываемся на смерть для анимации
        if (health != null)
        {
            health.onDeath += OnDeath;
        }
    }

    private void Update()
    {
        if (player == null || health == null || health.currentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (enemyType)
        {
            case EnemyType.Melee:
                UpdateMeleeBehavior(distance);
                break;
            case EnemyType.Ranged:
                UpdateRangedBehavior(distance);
                break;
        }

        // Обновляем анимацию движения
        UpdateMovementAnimation();
    }

    void UpdateMeleeBehavior(float distance)
    {
        if (distance > attackRange)
        {
            // Двигаемся к игроку
            MoveTowardsPlayer();

            // Анимация ходьбы
            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            // Останавливаемся
            if (animator != null)
                animator.SetBool("isWalking", false);

            // В зоне атаки
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void UpdateRangedBehavior(float distance)
    {
        // Дальний враг держит дистанцию
        if (distance > maxDistanceToPlayer)
        {
            // Приближаемся к игроку
            MoveTowardsPlayer();
            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else if (distance < minDistanceToPlayer)
        {
            // Отходим от игрока
            MoveAwayFromPlayer();
            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            // На оптимальной дистанции - стоим
            if (animator != null)
                animator.SetBool("isWalking", false);

            // Атакуем, если дистанция позволяет
            if (distance <= rangedAttackRange && Time.time >= nextAttackTime && !isAttacking)
            {
                RangedAttack();
                nextAttackTime = Time.time + rangedAttackCooldown;
            }
        }

        // Поворачиваемся к игроку
        LookAtPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Сохраняем Y координату
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        LookAtPlayer();
    }

    void MoveAwayFromPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    void AttackPlayer()
    {
        StartCoroutine(PerformMeleeAttack());
    }

    System.Collections.IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;

        // Анимация атаки
        if (animator != null)
            animator.SetTrigger("attack");

        // Небольшая задержка перед уроном (синхронизация с анимацией)
        yield return new WaitForSeconds(0.3f);

        // Проверяем, что игрок все еще в радиусе атаки
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Physical);
                Debug.Log($"👹 {gameObject.name} нанес {damage} урона ближним боем!");
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    void RangedAttack()
    {
        StartCoroutine(PerformRangedAttack());
    }

    System.Collections.IEnumerator PerformRangedAttack()
    {
        isAttacking = true;

        // Анимация атаки
        if (animator != null)
            animator.SetTrigger("attack");

        // Задержка для синхронизации с анимацией
        yield return new WaitForSeconds(0.2f);

        if (projectilePrefab != null && shootPoint != null)
        {
            // Создаем снаряд
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

            // Настраиваем компонент снаряда (ТОЛЬКО damage и speed!)
            EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
            if (projScript == null)
                projScript = projectile.AddComponent<EnemyProjectile>();

            projScript.damage = rangedDamage;
            projScript.speed = projectileSpeed;
            // projScript.target = player;      // ← МОЖНО УДАЛИТЬ
            // projScript.SetOwner(gameObject); // ← МОЖНО УДАЛИТЬ

            Debug.Log($"🏹 {gameObject.name} выпустил снаряд с уроном {rangedDamage}");
        }
        else
        {
            Debug.LogWarning($"❌ {gameObject.name}: Нет префаба снаряда или точки стрельбы!");
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    void UpdateMovementAnimation()
    {
        if (animator != null)
        {
            // Получаем текущую скорость движения
            float currentSpeed = GetComponent<Rigidbody>() != null ?
                GetComponent<Rigidbody>().linearVelocity.magnitude : speed;

            animator.SetFloat("speed", currentSpeed);
        }
    }

    void OnDeath()
    {
        if (animator != null)
            animator.SetTrigger("death");

        // Отключаем скрипт, чтобы враг не двигался после смерти
        enabled = false;

        // Уничтожаем объект через время (для проигрывания анимации смерти)
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyType == EnemyType.Melee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, maxDistanceToPlayer);
        }
    }
}