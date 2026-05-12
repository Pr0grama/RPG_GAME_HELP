using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float physicalDamage = 20f;
    public float magicalDamage = 30f;
    public Transform attackPoint;
    public float attackRange = 2f;
    public LayerMask enemyLayers;
    public GameObject magicProjectilePrefab;
    public Transform magicSpawnPoint;
    public MagicCooldownUI cooldownUI;

    [Header("Magic Cooldown")]
    public float magicCooldown = 2f;
    private float nextMagicTime = 0f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PhysicalAttack();
        }

        if (Input.GetMouseButtonDown(1) && Time.time >= nextMagicTime)
        {
            MagicalAttack();
            nextMagicTime = Time.time + magicCooldown;
            if (cooldownUI != null)
                cooldownUI.StartCooldown();
        }
    }

    void PhysicalAttack()
    {
        animator.SetTrigger("attack");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(physicalDamage, DamageType.Physical);
                Debug.Log("Нанесён физический урон: " + physicalDamage);
            }
        }
    }

    void MagicalAttack()
    {
        Debug.Log("✨ Магическая атака!");
        if (magicProjectilePrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicProjectilePrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);
        }
    }

    // ДОБАВИТЬ ЭТИ МЕТОДЫ:
    public float GetNextMagicTime()
    {
        return nextMagicTime;
    }

    public void SetNextMagicTime(float value)
    {
        nextMagicTime = value;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}