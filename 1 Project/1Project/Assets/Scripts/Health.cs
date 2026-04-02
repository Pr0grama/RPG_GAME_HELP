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

    public float deathAnimationDelay = 600f;

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

        Debug.Log($"💀 [HEALTH] {gameObject.name}: Die() вызван в {Time.time}");
        Debug.Log($"💀 [HEALTH] deathAnimationDelay = {deathAnimationDelay}");

        if (animator != null)
        {
            animator.SetTrigger("death");
            Debug.Log($"💀 [HEALTH] Анимация death запущена");
        }

        if (onDeath != null)
        {
            onDeath.Invoke();
            Debug.Log($"💀 [HEALTH] onDeath событие вызвано");
        }

        if (gameObject.CompareTag("Enemy"))
        {
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
                Debug.Log($"💀 [HEALTH] Физика заморожена");
            }

            float destroyTime = Time.time + deathAnimationDelay;
            Debug.Log($"⏰ [HEALTH] {gameObject.name}: будет уничтожен через {deathAnimationDelay} секунд (в {destroyTime})");

            Destroy(gameObject, deathAnimationDelay);
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"🗑️ [HEALTH] {gameObject.name}: OnDestroy вызван в {Time.time}!");
    }
}