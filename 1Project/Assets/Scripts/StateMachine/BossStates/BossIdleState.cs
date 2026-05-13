using StateMachine;
using UnityEngine;

namespace StateMachine.BossStates
{
    public class BossIdleState : IState
    {
        private BossContext context;

        public BossIdleState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.SetAnimationFloat("speed", 0f);
            context.SetAnimationBool("isWalking", false);
            Debug.Log($"👑 БОСС: состояние ПОКОЙ");
        }

        public void Update()
        {
            if (!context.isPeacefulMode)
            {
                // ✅ ИСПРАВЛЕНО
                context.ChangeState<BossAggroState>();
                return;
            }

            if (context.IsPlayerInRange(context.attackRange * 3f) && !context.isPeacefulMode)
            {
                // ✅ ИСПРАВЛЕНО
                context.ChangeState<BossAggroState>();
                return;
            }
        }

        public void Exit() { }
    }
}