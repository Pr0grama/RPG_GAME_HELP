using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 15f;
    private float damage;
    private ElementType element;
    private Transform target;
    private GameObject impactEffect;
    public bool isHeavy = false;

    public void Initialize(float dmg, ElementType elem, Transform aim, GameObject effect)
    {
        damage = dmg;
        element = elem;
        target = aim;
        impactEffect = effect;

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Magical);
                Debug.Log($"✨ Босс атакует {element}! Урон: {damage}, {(isHeavy ? "СИЛЬНАЯ" : "обычная")} атака");
            }

            if (impactEffect != null)
            {
                GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}