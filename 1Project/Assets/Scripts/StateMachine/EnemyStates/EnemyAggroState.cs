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
            Debug.Log($"🎯 {context.enemyTransform.name}: ВХОД В AGGRO!");
            context.SetAnimationBool("isWalking", true);
        }

        public void Update()
        {
            if (context.playerTransform == null) return;

            float distance = context.DistanceToPlayer();

            // Если нужно убегать — сразу уходим
            if (context.ShouldFlee())
            {
                context.ChangeState<EnemyFleeState>();
                return;
            }

            // Если игрок в радиусе атаки И можно атаковать — переходим в атаку
            if (context.IsPlayerInAttackRange() && context.CanAttack())
            {
                context.ChangeState<EnemyAttackState>();
                return;
            }

            // Иначе — продолжаем движение к игроку
            context.MoveTowardsPlayer();

            // Отладка
            Debug.Log($"🏃 {context.enemyTransform.name}: AGGRO | dist={distance:F1} | range={context.attackRange}");
        }

        public void Exit()
        {
            Debug.Log($"🎯 {context.enemyTransform.name}: ВЫХОД ИЗ AGGRO");
            context.SetAnimationBool("isWalking", false);
        }
    }
}