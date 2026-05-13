using UnityEngine;
using StateMachine;

namespace StateMachine.BossStates
{
    public class BossRangedAttackState : IState
    {
        private BossContext context;
        private float attackDuration = 0.6f;
        private float damageDelay = 0.3f;
        private float currentTime = 0f;
        private bool hasShot = false;

        public BossRangedAttackState(BossContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.TriggerAnimation("ranged_attack");
            currentTime = 0f;
            hasShot = false;
            Debug.Log($"👑 БОСС: состояние ДАЛЬНЯЯ АТАКА");
        }

        public void Update()
        {
            currentTime += Time.deltaTime;

            if (!hasShot && currentTime >= damageDelay)
            {
                // Выстрел
                context.PerformAttack();

                // Обновляем кулдаун дальней атаки
                var boss = context.bossTransform.GetComponent<BossStateMachine>();
                boss?.UseRangedAttack();

                hasShot = true;
            }

            if (currentTime >= attackDuration)
            {
                context.ChangeState<BossAggroState>();
            }
        }

        public void Exit() { }
    }
}