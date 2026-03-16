using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 30f;
    public float lifetime = 3f;
    public GameObject impactEffect;

    private void Start()
    {
        // Уничтожаем снаряд через время
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Двигаем вперёд (в локальном пространстве)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health enemyHealth = other.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, DamageType.Magical);
            Debug.Log("🧙 Урон магией: " + damage);
        }

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}