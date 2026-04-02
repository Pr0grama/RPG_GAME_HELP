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
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 15f;

    [Header("Настройки движения для дальнего боя")]
    public float minDistanceToPlayer = 5f;
    public float maxDistanceToPlayer = 8f;

    private float nextAttackTime = 0f;
    private Health health;
    private Animator animator;
    private bool isAttacking = false;

    private void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (shootPoint == null && enemyType == EnemyType.Ranged)
        {
            shootPoint = transform;
        }

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

        if (Time.frameCount % 60 == 0 && animator != null)
        {
            Debug.Log($"🎬 {gameObject.name}: speed в аниматоре = {animator.GetFloat("speed")}");
        }
    }

    void UpdateMeleeBehavior(float distance)
    {
        float currentSpeed = 0f;

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
            currentSpeed = speed; // Бежим к игроку
            Debug.Log($"🏃 {gameObject.name}: ДВИГАЕТСЯ, скорость = {currentSpeed}"); // ВРЕМЕННО
        }
        else
        {
            // В зоне атаки
            Debug.Log($"🧍 {gameObject.name}: СТОИТ, скорость = {currentSpeed}"); // ВРЕМЕННО
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        // Обновляем анимацию скорости
        UpdateMovementAnimation(currentSpeed);
    }

    void UpdateRangedBehavior(float distance)
    {
        float currentSpeed = 0f;

        if (distance > maxDistanceToPlayer)
        {
            MoveTowardsPlayer();
            currentSpeed = speed;
        }
        else if (distance < minDistanceToPlayer)
        {
            MoveAwayFromPlayer();
            currentSpeed = speed;
        }
        else
        {
            // На оптимальной дистанции - стоим
            currentSpeed = 0f;

            if (distance <= rangedAttackRange && Time.time >= nextAttackTime && !isAttacking)
            {
                RangedAttack();
                nextAttackTime = Time.time + rangedAttackCooldown;
            }
        }

        LookAtPlayer();

        // Обновляем анимацию скорости
        UpdateMovementAnimation(currentSpeed);
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
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

        if (animator != null)
            animator.SetTrigger("attack");

        // Останавливаем анимацию движения во время атаки
        UpdateMovementAnimation(0f);

        yield return new WaitForSeconds(0.3f);

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

        if (animator != null)
            animator.SetTrigger("attack");

        // Останавливаем анимацию движения во время атаки
        UpdateMovementAnimation(0f);

        yield return new WaitForSeconds(0.2f);

        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
            if (projScript == null)
                projScript = projectile.AddComponent<EnemyProjectile>();

            projScript.damage = rangedDamage;
            projScript.speed = projectileSpeed;

            Debug.Log($"🏹 {gameObject.name} выпустил снаряд с уроном {rangedDamage}");
        }
        else
        {
            Debug.LogWarning($"❌ {gameObject.name}: Нет префаба снаряда или точки стрельбы!");
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // Обновленный метод анимации - только speed
    void UpdateMovementAnimation(float currentSpeed)
    {
        if (animator != null)
        {
            // Нормализуем скорость (максимум = speed)
            float normalizedSpeed = currentSpeed / speed;
            animator.SetFloat("speed", normalizedSpeed);
        }
    }

    void OnDeath()
    {
        if (animator != null)
            animator.SetTrigger("death");

        enabled = false;
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