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

        // ✅ Добавляем контроллер оружия
        public BossWeaponController weaponController;

        public float rangedAttackRange;  // Добавить рядом с attackRange

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
            // ✅ Используем оружие для проверки кулдауна
            if (weaponController != null)
                return weaponController.CanAttack();
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public void PerformAttack()
        {
            // ✅ Используем систему оружия для атаки
            if (weaponController != null)
            {
                weaponController.PerformAttack(playerTransform);
                SetLastAttackTime();
            }
            else
            {
                DamagePlayer(false);
                SetLastAttackTime();
            }
        }

        public void PerformHeavyAttack()
        {
            if (weaponController != null)
            {
                weaponController.PerformHeavyAttack(playerTransform);
                SetLastHeavyAttackTime();
            }
            else
            {
                DamagePlayer(true);
                SetLastHeavyAttackTime();
            }
        }

        public bool CanHeavyAttack()
        {
            if (weaponController != null)
                return weaponController.CanHeavyAttack();
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

        public void SwitchToPhase2()
        {
            currentPhase = BossPhase.Phase2;
            attackCooldown = originalAttackCooldown * 0.5f;
            heavyAttackCooldown = originalHeavyAttackCooldown * 0.6f;
            speed = originalSpeed * 1.5f;

            if (weaponController != null)
            {
                weaponController.OnPhase2Start();
            }

            Debug.Log($"👑 БОСС ПЕРЕШЁЛ ВО 2 ФАЗУ! Скорость атак увеличена!");
        }

        public float GetHealthPercent()
        {
            if (health == null) return 1f;
            return health.currentHealth / health.maxHealth;
        }

        // ✅ Получение текущей стихии
        public ElementType GetCurrentElement()
        {
            return weaponController?.CurrentElement ?? ElementType.Fire;
        }

        // ✅ Получение текущего типа оружия
        public WeaponType GetCurrentWeaponType()
        {
            return weaponController?.CurrentWeaponType ?? WeaponType.Melee;
        }
    }

    public enum BossPhase
    {
        Phase1,
        Phase2
    }
}