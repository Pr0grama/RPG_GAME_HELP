using UnityEngine;
using StateMachine;

namespace StateMachine.BossStates
{
    public class BossContext
    {
        public Transform bossTransform;
        public Transform playerTransform;
        public Health health;
        public Animator animator;
        public BossStateMachine bossStateMachine;

        public float speed;
        public float originalSpeed;

        public float attackRange;
        public float attackDamage;
        public float attackCooldown;
        public float originalAttackCooldown;

        public float heavyAttackDamage;
        public float heavyAttackCooldown;
        public float originalHeavyAttackCooldown;

        private float lastAttackTime = -2f;
        private float lastHeavyAttackTime = -4f;

        public bool isPeacefulMode = true;
        public BossPhase currentPhase = BossPhase.Phase1;

        public void ChangeState<TState>() where TState : IState
        {
            bossStateMachine?.StateMachine?.ChangeState<TState>();
        }

        public void LookAtPlayer()
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - bossTransform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                bossTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void MoveTowardsPlayer()
        {
            if (playerTransform == null) return;
            Vector3 direction = (playerTransform.position - bossTransform.position).normalized;
            Vector3 newPosition = bossTransform.position + direction * speed * Time.deltaTime;
            newPosition.y = bossTransform.position.y;
            bossTransform.position = newPosition;
            LookAtPlayer();
        }

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public bool CanHeavyAttack()
        {
            return Time.time >= lastHeavyAttackTime + heavyAttackCooldown;
        }

        public void SetLastAttackTime()
        {
            lastAttackTime = Time.time;
        }

        public void SetLastHeavyAttackTime()
        {
            lastHeavyAttackTime = Time.time;
        }

        public bool IsPlayerInRange(float range)
        {
            if (playerTransform == null) return false;
            return Vector3.Distance(bossTransform.position, playerTransform.position) <= range;
        }

        public void DamagePlayer(bool isHeavy = false)
        {
            if (playerTransform == null) return;
            Health playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                float damage = isHeavy ? heavyAttackDamage : attackDamage;
                playerHealth.TakeDamage(damage, DamageType.Physical);
                Debug.Log($"👑 БОСС: нанёс {(isHeavy ? "СИЛЬНЫЙ" : "обычный")} урон {damage}");
            }
        }

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

        // ✅ ДОБАВИТЬ ЭТОТ МЕТОД
        public void SwitchToPhase2()
        {
            currentPhase = BossPhase.Phase2;
            attackCooldown = originalAttackCooldown * 0.5f;      // в 2 раза быстрее
            heavyAttackCooldown = originalHeavyAttackCooldown * 0.6f; // на 40% быстрее
            speed = originalSpeed * 1.5f;                       // на 50% быстрее

            Debug.Log($"👑 БОСС ПЕРЕШЁЛ ВО 2 ФАЗУ! " +
                      $"Атака: {attackCooldown}с (было {originalAttackCooldown}с), " +
                      $"Сильная атака: {heavyAttackCooldown}с (было {originalHeavyAttackCooldown}с), " +
                      $"Скорость: {speed} (было {originalSpeed})");
        }

        public float GetHealthPercent()
        {
            if (health == null) return 1f;
            return health.currentHealth / health.maxHealth;
        }
    }

    public enum BossPhase
    {
        Phase1,  // HP > 50%
        Phase2   // HP <= 50%
    }
}