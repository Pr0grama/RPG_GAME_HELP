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
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " погиб");
        Destroy(gameObject);
    }
}