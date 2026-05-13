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
    public System.Action onTakeDamage;  // Событие для получения урона

    public float deathAnimationDelay = 2f;  // Уменьшил до 2 секунд (было 600)

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

        // Вызываем событие получения урона
        onTakeDamage?.Invoke();

        // Анимация получения урона
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

        // Анимация смерти
        if (animator != null)
        {
            animator.SetTrigger("death");
            Debug.Log($"💀 [HEALTH] Анимация death запущена");
        }

        // Вызываем событие смерти
        if (onDeath != null)
        {
            onDeath.Invoke();
            Debug.Log($"💀 [HEALTH] onDeath событие вызвано");
        }

        if (gameObject.CompareTag("Enemy"))
        {
            // ✅ ОТКЛЮЧАЕМ EnemyStateMachine вместо EnemyAI
            EnemyStateMachine enemyStateMachine = GetComponent<EnemyStateMachine>();
            if (enemyStateMachine != null)
            {
                enemyStateMachine.enabled = false;
                Debug.Log($"💀 [HEALTH] EnemyStateMachine отключен");
            }

            // На всякий случай отключаем и старый EnemyAI (если есть)
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.enabled = false;
                Debug.Log($"💀 [HEALTH] EnemyAI отключен");
            }

            // Замораживаем физику
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
            }

            // Уничтожаем через время (для проигрывания анимации)
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