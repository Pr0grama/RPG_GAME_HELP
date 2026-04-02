using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 15f;
    public float lifetime = 5f;
    public GameObject impactEffect;

    private Vector3 direction;

    void Start()
    {
        // Летим вперед
        direction = transform.forward;

        // Уничтожаем через время
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // ПРОВЕРКА: если у объекта есть тег "Player" - наносим урон
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Magical);
                Debug.Log($"🏹 Снаряд попал в игрока! Урон: {damage}");
            }

            // Эффект попадания
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            // Уничтожаем снаряд
            Destroy(gameObject);
        }
        // Если попали в стену или другой объект (не врага) - просто уничтожаем
        else if (!other.CompareTag("Enemy"))
        {
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}