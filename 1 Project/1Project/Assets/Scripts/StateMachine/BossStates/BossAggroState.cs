using UnityEngine;
using StateMachine;

namespace StateMachine.BossStates
{
    public class BossAggroState : IState
    {
        private BossContext context;

        public BossAggroState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", context.speed);
            context.SetAnimationBool("isWalking", true);
            Debug.Log($"👑 БОСС: состояние АГРЕССИЯ");
        }

        public void Update()
        {
            if (context.playerTransform == null) return;
            if (context.health != null && context.health.currentHealth <= 0) return;

            if (context.IsPlayerInRange(context.attackRange))
            {
                if (context.CanHeavyAttack())
                {
                    // ✅ ИСПРАВЛЕНО
                    context.ChangeState<BossHeavyAttackState>();
                    return;
                }
                else if (context.CanAttack())
                {
                    // ✅ ИСПРАВЛЕНО
                    context.ChangeState<BossAttackState>();
                    return;
                }
                context.LookAtPlayer();
            }
            else
            {
                context.MoveTowardsPlayer();
            }
        }

        public void Exit()
        {
            context.SetAnimationBool("isWalking", false);
        }
    }
}