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

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, DamageType damageType)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " получил урон: " + amount + ", тип: " + damageType);

        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log("🔥 TakeDamage ВЫЗВАН на объекте: " + gameObject.name);
        currentHealth -= amount;
        Debug.Log(gameObject.name + " получил урон: " + amount + ", тип: " + damageType + ", осталось: " + currentHealth);
        // ...
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