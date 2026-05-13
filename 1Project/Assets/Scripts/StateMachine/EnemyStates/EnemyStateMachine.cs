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
    public EnemyStateContext Context => context;

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

        // Определяем isRanged
        if (!isRanged && enemyType == EnemyType.Ranged)
            isRanged = true;
        else if (isRanged && enemyType == EnemyType.Melee)
            isRanged = false;

        Debug.Log($"📊 {gameObject.name}: Тип={enemyType}, isRanged={isRanged}, isAggressive={isAggressive || globalAggressiveMode}");

        // === СОЗДАЁМ CONTEXT ===
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

        stateMachine = new StateMachine<EnemyStateMachine>(this);
        stateMachine.AddState(new EnemyIdleState(context));
        stateMachine.AddState(new EnemyAggroState(context));
        stateMachine.AddState(new EnemyAttackState(context));
        stateMachine.AddState(new EnemyFleeState(context));

        // === ЗАЩИТА ПАРАМЕТРОВ (самое важное) ===
        if (context.speed < 0.1f) context.speed = 3.5f;
        if (context.attackRange < 0.5f) context.attackRange = 2f;
        if (context.damage < 1f) context.damage = 10f;
        if (context.attackCooldown < 0.1f) context.attackCooldown = 1.5f;

        Debug.Log($"🛡️ {gameObject.name}: Защита параметров применена | speed={context.speed}");

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

        // Переключение глобального режима
        if (globalAggressiveMode != context.isAggressive)
        {
            Debug.Log($"🔄 {gameObject.name}: Смена режима! global={globalAggressiveMode}, context={context.isAggressive}");

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

        if (!context.isAggressive)
        {
            context.isAggressive = true;
            stateMachine?.ChangeState<EnemyAggroState>();
            Debug.Log($"⚔️ {gameObject.name}: получил урон, перехожу в AGGRO!");
            return;
        }

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

        // ✅ ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ ДЛЯ ВСЕХ ВРАГОВ
        EnemyStateMachine[] enemies = FindObjectsOfType<EnemyStateMachine>();
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.context != null)
            {
                enemy.context.isAggressive = globalAggressiveMode;
                if (globalAggressiveMode)
                {
                    enemy.stateMachine?.ChangeState<EnemyAggroState>();
                }
                else
                {
                    enemy.stateMachine?.ChangeState<EnemyIdleState>();
                }
            }
        }
    }

    public static void ToggleGlobalAggressiveMode()
    {
        SetGlobalAggressiveMode(!globalAggressiveMode);
    }

    public static bool GetGlobalAggressiveMode()
    {
        return globalAggressiveMode;
    }

    public void TriggerHitAnimation()
    {
        animator?.SetTrigger("hit");
    }
    public void ForceMoveToPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        Debug.Log($"🔥 ПРИНУДИТЕЛЬНОЕ ДВИЖЕНИЕ: {gameObject.name} -> {newPosition}");
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