using StateMachine;
using UnityEngine;

namespace StateMachine.BossStates
{
    public class BossAggroState : IState
    {
        private BossContext context;
        private float rangedAttackCooldown = 3f;
        private float lastRangedCheck;

        public BossAggroState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", context.speed);
            context.SetAnimationBool("isWalking", true);
            lastRangedCheck = Time.time;
            Debug.Log($"👑 БОСС: состояние АГРЕССИЯ");
        }

        public void Update()
        {
            if (context.playerTransform == null) return;
            if (context.health != null && context.health.currentHealth <= 0) return;

            float distance = Vector3.Distance(context.bossTransform.position, context.playerTransform.position);

            // 1. Проверяем дальнюю атаку (если игрок в радиусе и кулдаун прошёл)
            if (distance <= context.rangedAttackRange && distance > context.attackRange)
            {
                if (CanUseRangedAttack())
                {
                    context.ChangeState<BossRangedAttackState>();
                    return;
                }
            }

            // 2. Проверяем ближнюю атаку
            if (distance <= context.attackRange)
            {
                // Приоритет у сильной атаки
                if (context.CanHeavyAttack())
                {
                    context.ChangeState<BossHeavyAttackState>();
                    return;
                }
                else if (context.CanAttack())
                {
                    context.ChangeState<BossAttackState>();
                    return;
                }
                context.LookAtPlayer();
            }
            else
            {
                // Двигаемся к игроку
                context.MoveTowardsPlayer();
            }
        }

        private bool CanUseRangedAttack()
        {
            var boss = context.bossTransform.GetComponent<BossStateMachine>();
            return boss != null && Time.time >= boss.LastRangedAttackTime + rangedAttackCooldown;
        }

        public void Exit()
        {
            context.SetAnimationBool("isWalking", false);
        }
    }
}