using StateMachine;
using UnityEngine;

namespace StateMachine.EnemyStates
{
    public class EnemyIdleState : IState
    {
        private EnemyStateContext context;

        public EnemyIdleState(EnemyStateContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", 0f);
            context.SetAnimationBool("isWalking", false);
            Debug.Log($"{context.enemyTransform.name} -> IDLE (мирный)");
        }

        public void Update()
        {
            // Если включен агрессивный режим (через кнопку) - переходим в Aggro
            if (context.isAggressive)
            {
                Debug.Log($"{context.enemyTransform.name}: Агрессивный режим включен, перехожу в AGGRO!");
                context.ChangeState<EnemyAggroState>();
                return;
            }

            // В мирном режиме моб НЕ реагирует на игрока
            // Только проверяем, нужно ли убегать (если HP низкий)
            if (context.ShouldFlee())
            {
                Debug.Log($"{context.enemyTransform.name}: HP низкий, убегаю!");
                context.ChangeState<EnemyFleeState>();
            }
        }

        public void Exit() { }
    }
}