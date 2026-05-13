using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 15f;
    private float damage;
    private ElementType element;
    private Transform target;
    private bool isHeavy;
    private GameObject impactEffectPrefab;

    public void Initialize(float dmg, ElementType elem, Transform aim, bool heavy, GameObject impactPrefab)
    {
        damage = dmg;
        element = elem;
        target = aim;
        isHeavy = heavy;
        impactEffectPrefab = impactPrefab;

        // Меняем цвет снаряда в зависимости от стихии
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = GetElementColor();
        }

        // Меняем размер для сильной атаки
        if (isHeavy)
        {
            transform.localScale = Vector3.one * 1.5f;
        }

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
        transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Magical);
                Debug.Log($"✨ Босс атакует {element}! Урон: {damage}");
            }

            // Эффект попадания
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impact, 1f);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impact, 1f);
            }
            Destroy(gameObject);
        }
    }

    private Color GetElementColor()
    {
        switch (element)
        {
            case ElementType.Fire: return Color.red;
            case ElementType.Ice: return Color.cyan;
            case ElementType.Earth: return Color.green;
            case ElementType.Ether: return Color.magenta;
            default: return Color.white;
        }
    }
}