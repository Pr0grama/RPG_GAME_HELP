using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 15f;
    public float lifetime = 5f;
    public GameObject impactEffect;
    public Transform target; // Цель (игрок)

    private Vector3 direction;

    void Start()
    {
        if (target != null)
        {
            // Направляемся к игроку
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            // Если цели нет, летим вперед
            direction = transform.forward;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Поворачиваем снаряд в направлении движения
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, попали ли в игрока
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Magical);
                Debug.Log($"🏹 Снаряд врага нанес {damage} урона игроку!");
            }

            // Эффект попадания
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        // Столкновение с препятствиями
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}