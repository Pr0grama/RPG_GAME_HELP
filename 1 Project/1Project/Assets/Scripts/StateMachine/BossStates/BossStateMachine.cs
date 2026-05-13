using UnityEngine;
using StateMachine.BossStates;
using StateMachine;

public class BossStateMachine : MonoBehaviour
{
    [Header("Настройки босса")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float heavyAttackDamage = 40f;
    [SerializeField] private float heavyAttackCooldown = 4f;

    [Header("Настройки второй фазы (HP < 50%)")]
    [SerializeField] private float phase2SpeedMultiplier = 1.5f;
    [SerializeField] private float phase2AttackCooldownMultiplier = 0.5f;
    [SerializeField] private float phase2HeavyCooldownMultiplier = 0.6f;

    [Header("Система оружия")]
    [SerializeField] private WeaponType startWeapon = WeaponType.Melee;
    [SerializeField] private ElementType startElement = ElementType.Fire;
    [SerializeField] private bool autoInitializeWeapon = true;

    private Health health;
    private Animator animator;
    private Transform player;
    private BossContext context;
    private StateMachine<BossStateMachine> stateMachine;
    private BossWeaponController weaponController;

    private bool isPeacefulMode = true;
    private bool isPhase2Active = false;

    public StateMachine<BossStateMachine> StateMachine => stateMachine;
    public BossWeaponController WeaponController => weaponController;

    private void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // ✅ Инициализируем систему оружия
        weaponController = GetComponent<BossWeaponController>();
        if (weaponController == null)
        {
            weaponController = gameObject.AddComponent<BossWeaponController>();
        }

        if (autoInitializeWeapon)
        {
            weaponController.SetWeaponAndElement(startWeapon, startElement);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        context = new BossContext
        {
            bossTransform = transform,
            playerTransform = player,
            health = health,
            animator = animator,
            bossStateMachine = this,
            weaponController = weaponController,  // ✅ Передаём контроллер
            originalSpeed = speed,
            speed = speed,
            attackRange = attackRange,
            attackDamage = attackDamage,
            originalAttackCooldown = attackCooldown,
            attackCooldown = attackCooldown,
            heavyAttackDamage = heavyAttackDamage,
            originalHeavyAttackCooldown = heavyAttackCooldown,
            heavyAttackCooldown = heavyAttackCooldown,
            isPeacefulMode = isPeacefulMode
        };

        stateMachine = new StateMachine<BossStateMachine>(this);
        stateMachine.AddState(new BossIdleState(context));
        stateMachine.AddState(new BossAggroState(context));
        stateMachine.AddState(new BossAttackState(context));
        stateMachine.AddState(new BossHeavyAttackState(context));

        stateMachine.ChangeState<BossIdleState>();

        if (health != null)
        {
            health.onTakeDamage += OnTakeDamage;
        }

        Debug.Log($"👑 Босс инициализирован. Оружие: {startWeapon}, Стихия: {startElement}");
    }

    private void Update()
    {
        stateMachine?.Update();
        CheckPhaseTransition();
    }

    private void OnTakeDamage()
    {
        if (isPeacefulMode)
        {
            isPeacefulMode = false;
            context.isPeacefulMode = false;
            stateMachine?.ChangeState<BossAggroState>();
            Debug.Log($"👑 БОСС: получил урон, вышел из мирного режима!");
        }
    }

    private void CheckPhaseTransition()
    {
        if (health == null || isPhase2Active) return;

        float healthPercent = health.currentHealth / health.maxHealth;

        if (healthPercent <= 0.5f)
        {
            isPhase2Active = true;
            context.SwitchToPhase2();
            Debug.Log($"👑 БОСС: ПЕРЕХОД ВО ВТОРУЮ ФАЗУ! (HP = {healthPercent:P0})");
        }
    }

    private void OnDestroy()
    {
        if (health != null && health.onTakeDamage != null)
        {
            health.onTakeDamage -= OnTakeDamage;
        }
    }
}