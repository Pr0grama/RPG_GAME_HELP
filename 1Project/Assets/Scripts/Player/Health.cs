using UnityEngine;
using System.Collections;

public enum DamageType
{
    Physical,
    Magical
}

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public System.Action onDeath;
    public System.Action onTakeDamage;

    public float deathAnimationDelay = 2f;

    private Animator animator;
    private bool isDead = false;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 0.5f;

    private Rigidbody rb;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(float amount, DamageType damageType)
    {
        if (isDead) return;
        if (isInvulnerable) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} получил урон: {amount}, осталось: {currentHealth}");

        onTakeDamage?.Invoke();

        if (animator != null)
        {
            animator.SetTrigger("hit");
        }

        StartCoroutine(InvulnerabilityCoroutine());

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 [HEALTH] {gameObject.name}: Die() вызван");

        if (animator != null)
        {
            animator.SetTrigger("death");
            Debug.Log($"💀 [HEALTH] Анимация death запущена");
        }

        // ✅ ДОБАВЛЕНО: Увеличиваем счётчик убийств
        if (gameObject.CompareTag("Enemy") && GameStats.Instance != null)
        {
            GameStats.Instance.AddKill();
            Debug.Log($"📊 Добавлено убийство. Всего: {GameStats.Instance.KillCount}");
        }

        if (onDeath != null)
        {
            onDeath.Invoke();
            Debug.Log($"💀 [HEALTH] onDeath событие вызвано");
        }

        if (gameObject.CompareTag("Enemy"))
        {
            EnemyStateMachine enemyStateMachine = GetComponent<EnemyStateMachine>();
            if (enemyStateMachine != null)
            {
                enemyStateMachine.enabled = false;
                Debug.Log($"💀 [HEALTH] EnemyStateMachine отключен");
            }

            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.enabled = false;
                Debug.Log($"💀 [HEALTH] EnemyAI отключен");
            }

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
            }

            Destroy(gameObject, deathAnimationDelay);
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"🗑️ [HEALTH] {gameObject.name}: OnDestroy");
    }

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        Debug.Log($"❤️ SetHealth: новое HP = {currentHealth}");
    }
}