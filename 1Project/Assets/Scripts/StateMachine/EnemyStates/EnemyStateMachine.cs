using UnityEngine;
using StateMachine.EnemyStates;
using StateMachine;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("Основные настройки")]
    public EnemyType enemyType = EnemyType.Melee;
    public float speed = 3f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;
    public float fleeHealthThreshold = 0.3f;

    [Header("Режимы")]
    public bool isAggressive = false;

    [Header("Для дальнего боя (опционально)")]
    public bool isRanged = false;
    public float rangedAttackRange = 10f;
    public float rangedAttackCooldown = 2f;
    public float rangedDamage = 15f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 15f;
    public float minDistanceToPlayer = 5f;
    public float maxDistanceToPlayer = 8f;

    private Health health;
    private Animator animator;
    private Transform player;
    private EnemyStateContext context;
    private StateMachine<EnemyStateMachine> stateMachine;
    private bool isDead = false;

    private static bool globalAggressiveMode = false;

    public StateMachine<EnemyStateMachine> StateMachine => stateMachine;

    private void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"✅ {gameObject.name}: Игрок найден - {player.name}");
        }
        else
        {
            Debug.LogError($"❌ {gameObject.name}: Игрок НЕ НАЙДЕН!");
            return;
        }

        // Устанавливаем isRanged на основе enemyType (но только если не задано вручную)
        if (!isRanged && enemyType == EnemyType.Ranged)
            isRanged = true;
        else if (isRanged && enemyType == EnemyType.Melee)
            isRanged = false;

        Debug.Log($"📊 {gameObject.name}: Тип={enemyType}, isRanged={isRanged}, isAggressive={isAggressive || globalAggressiveMode}");

        // Создаём контекст
        context = new EnemyStateContext
        {
            enemyTransform = transform,
            playerTransform = player,
            health = health,
            animator = animator,
            speed = speed,
            attackRange = attackRange,
            attackCooldown = attackCooldown,
            damage = damage,
            fleeHealthThreshold = fleeHealthThreshold,
            isAggressive = isAggressive || globalAggressiveMode,
            enemyStateMachine = this,
            isRanged = isRanged,
            rangedAttackRange = rangedAttackRange,
            rangedAttackCooldown = rangedAttackCooldown,
            rangedDamage = rangedDamage,
            projectilePrefab = projectilePrefab,
            shootPoint = shootPoint,
            projectileSpeed = projectileSpeed,
            minDistanceToPlayer = minDistanceToPlayer,
            maxDistanceToPlayer = maxDistanceToPlayer
        };

        // Создаём машину состояний
        stateMachine = new StateMachine<EnemyStateMachine>(this);
        stateMachine.AddState(new EnemyIdleState(context));
        stateMachine.AddState(new EnemyAggroState(context));
        stateMachine.AddState(new EnemyAttackState(context));
        stateMachine.AddState(new EnemyFleeState(context));

        // Начальное состояние
        if (context.isAggressive)
        {
            stateMachine.ChangeState<EnemyAggroState>();
            Debug.Log($"⚔️ {gameObject.name}: начальное состояние AGGRO");
        }
        else
        {
            stateMachine.ChangeState<EnemyIdleState>();
            Debug.Log($"😴 {gameObject.name}: начальное состояние IDLE");
        }

        if (health != null)
        {
            health.onTakeDamage += OnTakeDamage;
            health.onDeath += OnDeath;
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (player == null) return;

        stateMachine?.Update();

        if (globalAggressiveMode != context.isAggressive)
        {
            context.isAggressive = globalAggressiveMode;
            if (globalAggressiveMode)
            {
                stateMachine?.ChangeState<EnemyAggroState>();
                Debug.Log($"⚔️ {gameObject.name}: переключен в АГРЕССИВНЫЙ режим!");
            }
            else
            {
                stateMachine?.ChangeState<EnemyIdleState>();
                Debug.Log($"😴 {gameObject.name}: переключен в МИРНЫЙ режим!");
            }
        }
    }

    private void OnTakeDamage()
    {
        if (isDead) return;

        Debug.Log($"💥 {gameObject.name}: получил урон");

        // Если мирный режим - переключаем в агрессию при получении урона
        if (!context.isAggressive)
        {
            context.isAggressive = true;
            stateMachine?.ChangeState<EnemyAggroState>();
            Debug.Log($"⚔️ {gameObject.name}: получил урон, перехожу в AGGRO!");
            return;
        }

        // Проверяем, нужно ли убегать (при низком HP)
        if (context.ShouldFlee())
        {
            stateMachine?.ChangeState<EnemyFleeState>();
        }
    }

    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 {gameObject.name}: смерть, проигрываю анимацию");

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public static void SetGlobalAggressiveMode(bool aggressive)
    {
        globalAggressiveMode = aggressive;
        Debug.Log($"🌍 Глобальный режим мобов изменён на: {(aggressive ? "АГРЕССИВНЫЙ" : "МИРНЫЙ")}");
    }

    public static void ToggleGlobalAggressiveMode()
    {
        SetGlobalAggressiveMode(!globalAggressiveMode);
    }

    public void TriggerHitAnimation()
    {
        animator?.SetTrigger("hit");
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            if (health.onTakeDamage != null) health.onTakeDamage -= OnTakeDamage;
            if (health.onDeath != null) health.onDeath -= OnDeath;
        }
    }
}