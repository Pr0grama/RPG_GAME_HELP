using UnityEngine;
using StateMachine;

namespace StateMachine.BossStates
{
    public class BossAttackState : IState
    {
        private BossContext context;
        private float attackDuration = 0.8f;
        private float damageDelay = 0.4f;
        private float currentTime = 0f;
        private bool hasDamaged = false;

        public BossAttackState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.TriggerAnimation("attack");
            context.SetLastAttackTime();
            currentTime = 0f;
            hasDamaged = false;
            Debug.Log($"👑 БОСС: состояние ОБЫЧНАЯ АТАКА");
        }

        public void Update()
        {
            currentTime += Time.deltaTime;

            if (!hasDamaged && currentTime >= damageDelay)
            {
                if (context.IsPlayerInRange(context.attackRange + 0.5f))
                {
                    context.PerformAttack();
                }
                hasDamaged = true;
            }

            if (currentTime >= attackDuration)
            {
                context.ChangeState<BossAggroState>();
            }
        }

        public void Exit() { }
    }
}