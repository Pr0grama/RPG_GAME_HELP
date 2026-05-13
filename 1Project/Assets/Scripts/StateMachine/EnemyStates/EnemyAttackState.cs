using UnityEngine;
using StateMachine;

namespace StateMachine.EnemyStates
{
    public class EnemyAttackState : IState
    {
        private EnemyStateContext context;
        private float attackTimer = 0.5f;
        private bool hasDamaged = false;
        private bool hasShot = false;

        public EnemyAttackState(EnemyStateContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.TriggerAnimation("attack");
            context.SetLastAttackTime();
            attackTimer = 0.5f;
            hasDamaged = false;
            hasShot = false;
            Debug.Log($"{context.enemyTransform.name} -> ATTACK");
        }

        public void Update()
        {
            attackTimer -= Time.deltaTime;

            // Для ближнего боя - урон в середине анимации
            if (!context.isRanged && attackTimer <= 0.25f && !hasDamaged)
            {
                if (context.DistanceToPlayer() <= context.attackRange)  // ✅ ИСПРАВЛЕНО
                {
                    context.DamagePlayer();
                }
                hasDamaged = true;
            }

            // Для дальнего боя - выстрел в середине анимации
            if (context.isRanged && attackTimer <= 0.25f && !hasShot)
            {
                context.RangedAttack();
                hasShot = true;
            }

            if (attackTimer <= 0f)
            {
                context.ChangeState<EnemyAggroState>();
            }
        }

        public void Exit() { }
    }
}