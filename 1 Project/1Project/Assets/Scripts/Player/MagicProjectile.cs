using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 30f;
    public float lifetime = 3f;
    public GameObject impactEffect;
    public bool isMagic = true;  // true - магия, false - физическая атака

    private Vector3 direction;

    private void Start()
    {
        direction = transform.forward;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Вращаем снаряд в направлении движения
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Попадание во врага
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                DamageType type = isMagic ? DamageType.Magical : DamageType.Physical;
                enemyHealth.TakeDamage(damage, type);

                string attackType = isMagic ? "магией" : "физической атакой";
                Debug.Log($"🏹 Снаряд нанёс {damage} урона {attackType}!");
            }

            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        // Попадание в стену или препятствие
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            if (impactEffect != null)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}