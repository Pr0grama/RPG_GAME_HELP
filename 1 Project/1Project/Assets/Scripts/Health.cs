using UnityEngine;

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

    // Ссылка на скрипт анимаций игрока
    private PlayerAnimations playerAnimations;

    private void Start()
    {
        currentHealth = maxHealth;

        // Пытаемся получить компонент анимаций (если он есть на этом объекте)
        playerAnimations = GetComponent<PlayerAnimations>();

        // Если не нашли на этом объекте, ищем в детях (на случай, если аниматор на модели)
        if (playerAnimations == null)
        {
            playerAnimations = GetComponentInChildren<PlayerAnimations>();
        }
    }

    public void TakeDamage(float amount, DamageType damageType)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " получил урон: " + amount + ", тип: " + damageType);

        // ✅ Запускаем анимацию получения урона
        if (playerAnimations != null)
        {
            playerAnimations.TriggerHitAnimation();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " погиб");

        if (onDeath != null)
            onDeath.Invoke();

        // Для врагов — уничтожить
        if (gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        // Для игрока — не уничтожаем, а активируем Game Over
    }
}