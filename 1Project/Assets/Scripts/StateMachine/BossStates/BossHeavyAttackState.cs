using UnityEngine;
using StateMachine;

namespace StateMachine.BossStates
{
    public class BossHeavyAttackState : IState
    {
        private BossContext context;
        private float attackDuration = 1.2f;
        private float damageDelay = 0.6f;
        private float extraRange = 1.5f;
        private float currentTime = 0f;
        private bool hasDamaged = false;

        public BossHeavyAttackState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.TriggerAnimation("heavy_attack");
            context.SetLastHeavyAttackTime();
            currentTime = 0f;
            hasDamaged = false;
            Debug.Log($"👑 БОСС: состояние СИЛЬНАЯ АТАКА");
        }

        public void Update()
        {
            currentTime += Time.deltaTime;

           
            if (!hasDamaged && currentTime >= damageDelay)
            {
                if (context.IsPlayerInRange(context.attackRange + extraRange))
                {
                    context.PerformHeavyAttack();
                }
                hasDamaged = true;
            }

            if (currentTime >= attackDuration)
            {
                // ✅ ИСПРАВЛЕНО
                context.ChangeState<BossAggroState>();
            }
        }

        public void Exit() { }
    }
}