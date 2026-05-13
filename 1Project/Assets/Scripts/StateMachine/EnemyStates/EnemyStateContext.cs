using UnityEngine;
using StateMachine;

namespace StateMachine.EnemyStates
{
    public class EnemyStateContext
    {
        // Основные ссылки
        public Transform enemyTransform;
        public Transform playerTransform;
        public Health health;
        public Animator animator;
        public EnemyStateMachine enemyStateMachine;

        // Настройки ближнего боя
        public float speed;
        public float attackRange;
        public float attackCooldown;
        public float damage;
        public float fleeHealthThreshold = 0.3f;

        // Настройки дальнего боя
        public bool isRanged = false;
        public float rangedAttackRange = 10f;
        public float rangedAttackCooldown = 2f;
        public float rangedDamage = 15f;
        public GameObject projectilePrefab;
        public Transform shootPoint;
        public float projectileSpeed = 15f;
        public float minDistanceToPlayer = 5f;
        public float maxDistanceToPlayer = 8f;

        private float lastAttackTime;
        private float lastRangedAttackTime;

        // Режим: true - агрессивный, false - мирный
        public bool isAggressive = false;

        // Метод для смены состояния
        public void ChangeState<TState>() where TState : IState
        {
            enemyStateMachine?.StateMachine?.ChangeState<TState>();
        }

        // Получить дистанцию до игрока
        public float DistanceToPlayer()
        {
            if (playerTransform == null) return Mathf.Infinity;
            return Vector3.Distance(enemyTransform.position, playerTransform.position);
        }

        // Поворот к игроку
        public void LookAtPlayer()
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - enemyTransform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                enemyTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // Движение к игроку
        // Движение к игроку
        public void MoveTowardsPlayer()
        {
            if (playerTransform == null) return;

            Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;
            Vector3 newPosition = enemyTransform.position + direction * speed * Time.deltaTime;
            newPosition.y = enemyTransform.position.y;  // Сохраняем высоту
            enemyTransform.position = newPosition;

            // Поворачиваемся к игроку
            LookAtPlayer();

            // Отладка
            Debug.Log($"🏃 {enemyTransform.name}: MoveTowardsPlayer, speed={speed}, newPos={newPosition}");
        }

        // Движение от игрока (для убегания)
        public void MoveAwayFromPlayer()
        {
            if (playerTransform == null) return;

            // Направление от игрока
            Vector3 direction = (enemyTransform.position - playerTransform.position).normalized;
            Vector3 newPosition = enemyTransform.position + direction * speed * Time.deltaTime;
            newPosition.y = enemyTransform.position.y;
            enemyTransform.position = newPosition;

            // ✅ ПОВОРАЧИВАЕМСЯ В СТОРОНУ ДВИЖЕНИЯ, а не к игроку!
            if (direction != Vector3.zero)
            {
                enemyTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // Проверка, можно ли атаковать
        public bool CanAttack()
        {
            float cooldown = isRanged ? rangedAttackCooldown : attackCooldown;
            float lastTime = isRanged ? lastRangedAttackTime : lastAttackTime;
            return Time.time >= lastTime + cooldown;
        }

        // Установить время последней атаки
        public void SetLastAttackTime()
        {
            if (isRanged)
                lastRangedAttackTime = Time.time;
            else
                lastAttackTime = Time.time;
        }

        // Проверка, в радиусе ли игрок для атаки
        public bool IsPlayerInAttackRange()
        {
            float range = isRanged ? rangedAttackRange : attackRange;
            return DistanceToPlayer() <= range;
        }

        // Проверка, нужно ли убегать (только при низком HP)
        public bool ShouldFlee()
        {
            if (health == null) return false;
            float healthPercent = health.currentHealth / health.maxHealth;
            return healthPercent <= fleeHealthThreshold;
        }

        // Нанесение урона игроку (ближний бой)
        public void DamagePlayer()
        {
            if (playerTransform == null) return;
            Health playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Physical);
                Debug.Log($"👹 {enemyTransform.name} нанёс {damage} урона!");
            }
        }

        // Дальняя атака
        public void RangedAttack()
        {
            if (projectilePrefab != null && shootPoint != null && playerTransform != null)
            {
                GameObject projectile = Object.Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
                EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
                if (projScript == null)
                    projScript = projectile.AddComponent<EnemyProjectile>();

                projScript.damage = rangedDamage;
                projScript.speed = projectileSpeed;

                Vector3 direction = (playerTransform.position - shootPoint.position).normalized;
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.linearVelocity = direction * projectileSpeed;
            }
        }

        // Управление дистанцией для дальнего боя
        public void ManageRangeDistance()
        {
            float distance = DistanceToPlayer();

            if (distance > maxDistanceToPlayer)
            {
                MoveTowardsPlayer();
            }
            else if (distance < minDistanceToPlayer)
            {
                MoveAwayFromPlayer();
            }
            else
            {
                LookAtPlayer();
            }
        }

        // Анимации
        public void TriggerAnimation(string trigger)
        {
            animator?.SetTrigger(trigger);
        }

        public void SetAnimationFloat(string name, float value)
        {
            animator?.SetFloat(name, value);
        }

        public void SetAnimationBool(string name, bool value)
        {
            animator?.SetBool(name, value);
        }
    }
}