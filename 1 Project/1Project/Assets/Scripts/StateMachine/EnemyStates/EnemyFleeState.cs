using StateMachine;
using UnityEngine;

namespace StateMachine.EnemyStates
{
    public class EnemyFleeState : IState
    {
        private EnemyStateContext context;

        public EnemyFleeState(EnemyStateContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", context.speed);
            context.SetAnimationBool("isWalking", true);
            Debug.Log($"{context.enemyTransform.name} -> FLEE (убегает)");
        }

        public void Update()
        {
            // Убегаем от игрока
            context.MoveAwayFromPlayer();

            // Если здоровье восстановилось или агрессивный режим включен - возвращаемся
            if (!context.ShouldFlee())
            {
                if (context.isAggressive)
                    context.ChangeState<EnemyAggroState>();
                else
                    context.ChangeState<EnemyIdleState>();
            }
        }

        public void Exit()
        {
            context.SetAnimationBool("isWalking", false);
        }
    }
}