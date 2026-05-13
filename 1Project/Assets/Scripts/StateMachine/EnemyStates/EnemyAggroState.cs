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
            Debug.Log($"{context.enemyTransform.name} -> AGGRO (скорость={context.speed})");
        }

        public void Update()
        {
            // Проверяем, жив ли игрок
            if (context.playerTransform == null)
            {
                Debug.LogWarning($"{context.enemyTransform.name}: playerTransform = null!");
                return;
            }

            float distance = context.DistanceToPlayer();

            // Проверка на бегство при низком HP
            if (context.ShouldFlee())
            {
                Debug.Log($"{context.enemyTransform.name}: HP низкий, убегаю!");
                context.ChangeState<EnemyFleeState>();
                return;
            }

            // Проверка, можно ли атаковать
            if (context.IsPlayerInAttackRange() && context.CanAttack())
            {
                Debug.Log($"{context.enemyTransform.name}: В радиусе атаки, атакую! (дистанция={distance:F1})");
                context.ChangeState<EnemyAttackState>();
                return;
            }

            // ✅ ДВИЖЕНИЕ К ИГРОКУ
            if (context.isRanged)
            {
                // Для дальнего боя - управление дистанцией
                context.ManageRangeDistance();
                Debug.Log($"{context.enemyTransform.name}: Управление дистанцией, движение к игроку");
            }
            else
            {
                // Для ближнего боя - просто бежим к игроку
                context.MoveTowardsPlayer();
                Debug.Log($"{context.enemyTransform.name}: Движение к игроку, дистанция={distance:F1}");
            }
        }

        public void Exit()
        {
            context.SetAnimationBool("isWalking", false);
        }
    }
}