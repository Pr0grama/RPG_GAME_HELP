using StateMachine;
using UnityEngine;

namespace StateMachine.EnemyStates
{
    public class EnemyAggroState : IState
    {
        private EnemyStateContext context;

        public EnemyAggroState(EnemyStateContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", context.speed);
            context.SetAnimationBool("isWalking", true);
            Debug.Log($"{context.enemyTransform.name} -> AGGRO");
        }

        public void Update()
        {
            // Проверка на бегство при низком HP
            if (context.ShouldFlee())
            {
                Debug.Log($"{context.enemyTransform.name}: HP низкий, убегаю!");
                context.ChangeState<EnemyFleeState>();
                return;
            }

            // Если агрессивный режим выключен - возвращаемся в Idle
            if (!context.isAggressive)
            {
                Debug.Log($"{context.enemyTransform.name}: Агрессивный режим выключен, возвращаюсь в IDLE");
                context.ChangeState<EnemyIdleState>();
                return;
            }

            // Проверка, можно ли атаковать
            if (context.IsPlayerInAttackRange() && context.CanAttack())
            {
                context.ChangeState<EnemyAttackState>();
                return;
            }

            // Движение к игроку (для ближнего) или управление дистанцией (для дальнего)
            if (context.isRanged)
            {
                context.ManageRangeDistance();
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