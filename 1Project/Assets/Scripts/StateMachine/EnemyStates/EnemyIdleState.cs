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
            Debug.Log($"{context.enemyTransform.name} -> IDLE");
        }

        public void Update()
        {
            // ✅ ПРИНУДИТЕЛЬНЫЙ ПЕРЕХОД В AGGRO ДЛЯ ТЕСТА
            Debug.Log($"{context.enemyTransform.name}: IDLE Update, isAggressive={context.isAggressive}");

            if (context.isAggressive)
            {
                Debug.Log($"🔥 {context.enemyTransform.name}: ПЕРЕХОЖУ В AGGRO!");
                context.ChangeState<EnemyAggroState>();
                return;
            }

            // Если игрок рядом - тоже переходим
            if (context.playerTransform != null && context.DistanceToPlayer() < 10f)
            {
                Debug.Log($"🔥 {context.enemyTransform.name}: ИГРОК РЯДОМ, ПЕРЕХОЖУ В AGGRO!");
                context.isAggressive = true;
                context.ChangeState<EnemyAggroState>();
            }
        }

        public void Exit() { }
    }
}