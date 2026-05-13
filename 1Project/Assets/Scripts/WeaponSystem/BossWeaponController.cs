using UnityEngine;
using System.Collections;

public class BossWeaponController : MonoBehaviour
{
    [Header("Текущее оружие и стихия")]
    public WeaponType currentWeaponType = WeaponType.Melee;
    public ElementType currentElement = ElementType.Fire;

    [Header("Настройки атак")]
    public float meleeDamage = 30f;
    public float heavyMeleeDamage = 50f;
    public float rangedDamage = 25f;
    public float heavyRangedDamage = 40f;
    public float meleeRange = 3f;
    public float attackCooldown = 2f;

    [Header("Ближний бой (общие)")]
    public GameObject meleeEffectPrefab;      // Один эффект для всех ударов
    public Transform meleeEffectPoint;
    public AudioClip meleeSound;              // Один звук для ближнего боя
    public AudioClip heavyMeleeSound;         // Звук для сильной атаки

    [Header("Дальний бой (общие)")]
    public Transform shootPoint;
    public AudioClip rangedSound;             // Один звук для всех выстрелов
    public AudioClip heavyRangedSound;        // Звук для сильного выстрела

    [Header("Снаряды для разных стихий (дальний бой)")]
    public GameObject fireProjectilePrefab;   // Красный снаряд
    public GameObject iceProjectilePrefab;    // Голубой снаряд
    public GameObject earthProjectilePrefab;  // Зелёный снаряд
    public GameObject etherProjectilePrefab;  // Фиолетовый снаряд

    [Header("Эффекты попадания для разных стихий (дальний бой)")]
    public GameObject fireImpactPrefab;
    public GameObject iceImpactPrefab;
    public GameObject earthImpactPrefab;
    public GameObject etherImpactPrefab;

    private float lastAttackTime = -2f;
    private AudioSource audioSource;
    private Transform player;

    public WeaponType CurrentWeaponType => currentWeaponType;
    public ElementType CurrentElement => currentElement;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        ApplyElementVisual();
        Debug.Log($"👑 BossWeaponController инициализирован. Оружие: {currentWeaponType}, Стихия: {currentElement}");
    }

    private void Update()
    {
        if (Time.frameCount % 600 == 0)
        {
            ChangeToRandomElement();
        }
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public bool CanHeavyAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown * 2;
    }

    public void PerformAttack(Transform target)
    {
        if (!CanAttack()) return;
        lastAttackTime = Time.time;

        if (currentWeaponType == WeaponType.Melee)
            PerformMeleeAttack(target, false);
        else
            PerformRangedAttack(target, false);
    }

    public void PerformHeavyAttack(Transform target)
    {
        if (!CanHeavyAttack()) return;
        lastAttackTime = Time.time;

        if (currentWeaponType == WeaponType.Melee)
            PerformMeleeAttack(target, true);
        else
            PerformRangedAttack(target, true);
    }

    // ==================== БЛИЖНИЙ БОЙ ====================
    private void PerformMeleeAttack(Transform target, bool isHeavy)
    {
        if (target == null) return;

        float damage = isHeavy ? heavyMeleeDamage : meleeDamage;
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= meleeRange)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, DamageType.Physical);
                Debug.Log($"👑 Босс атакует! Урон: {damage}, {(isHeavy ? "СИЛЬНАЯ" : "обычная")} атака");
            }
        }

        // Визуальный эффект (один для всех)
        CreateMeleeEffect();

        // Звук (один для всех, но отдельный для сильной атаки)
        AudioClip sound = isHeavy ? heavyMeleeSound : meleeSound;
        if (sound != null) audioSource.PlayOneShot(sound);
    }

    private void CreateMeleeEffect()
    {
        if (meleeEffectPrefab == null) return;

        Vector3 effectPos = meleeEffectPoint != null ? meleeEffectPoint.position : transform.position + transform.forward * 2;
        GameObject effect = Instantiate(meleeEffectPrefab, effectPos, transform.rotation);
        Destroy(effect, 0.5f);
    }

    // ==================== ДАЛЬНИЙ БОЙ ====================
    private void PerformRangedAttack(Transform target, bool isHeavy)
    {
        if (target == null) return;

        GameObject prefab = GetProjectilePrefabByElement();
        if (prefab == null)
        {
            Debug.LogError($"❌ Нет префаба снаряда для стихии {currentElement}!");
            return;
        }

        if (shootPoint == null) shootPoint = transform;

        float damage = isHeavy ? heavyRangedDamage : rangedDamage;

        // Создаём снаряд
        GameObject projectile = Instantiate(prefab, shootPoint.position, shootPoint.rotation);
        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript == null) projScript = projectile.AddComponent<BossProjectile>();

        // Передаём параметры
        GameObject impactPrefab = GetImpactPrefabByElement();
        projScript.Initialize(damage, currentElement, target, isHeavy, impactPrefab);

        // Звук (один для всех, но отдельный для сильной атаки)
        AudioClip sound = isHeavy ? heavyRangedSound : rangedSound;
        if (sound != null) audioSource.PlayOneShot(sound);

        Debug.Log($"🏹 Босс выпускает {currentElement} снаряд! Урон: {damage}");
    }

    private GameObject GetProjectilePrefabByElement()
    {
        switch (currentElement)
        {
            case ElementType.Fire: return fireProjectilePrefab;
            case ElementType.Ice: return iceProjectilePrefab;
            case ElementType.Earth: return earthProjectilePrefab;
            case ElementType.Ether: return etherProjectilePrefab;
            default: return fireProjectilePrefab;
        }
    }

    private GameObject GetImpactPrefabByElement()
    {
        switch (currentElement)
        {
            case ElementType.Fire: return fireImpactPrefab;
            case ElementType.Ice: return iceImpactPrefab;
            case ElementType.Earth: return earthImpactPrefab;
            case ElementType.Ether: return etherImpactPrefab;
            default: return null;
        }
    }

    // ==================== СТИХИИ ====================
    public void ChangeToRandomElement()
    {
        ElementType newElement = GetRandomElement();
        if (newElement != currentElement)
        {
            currentElement = newElement;
            ApplyElementVisual();
            Debug.Log($"✨ Босс сменил стихию на: {currentElement}");
        }
    }

    public void ChangeElement(ElementType newElement)
    {
        currentElement = newElement;
        ApplyElementVisual();
        Debug.Log($"✨ Босс сменил стихию на: {currentElement}");
    }

    private void ApplyElementVisual()
    {
        // Меняем цвет свечения босса в зависимости от стихии
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color elementColor = GetElementColor();

        foreach (Renderer rend in renderers)
        {
            if (rend.material != null)
            {
                rend.material.SetColor("_EmissionColor", elementColor * 0.5f);
            }
        }
    }

    private Color GetElementColor()
    {
        switch (currentElement)
        {
            case ElementType.Fire: return new Color(1f, 0.3f, 0.1f);
            case ElementType.Ice: return new Color(0.2f, 0.6f, 1f);
            case ElementType.Earth: return new Color(0.3f, 0.7f, 0.2f);
            case ElementType.Ether: return new Color(0.8f, 0.2f, 0.8f);
            default: return Color.white;
        }
    }

    private ElementType GetRandomElement()
    {
        System.Array values = System.Enum.GetValues(typeof(ElementType));
        return (ElementType)values.GetValue(Random.Range(0, values.Length));
    }

    public void SwitchWeapon(WeaponType newType)
    {
        if (currentWeaponType == newType) return;

        currentWeaponType = newType;
        Debug.Log($"⚔️ Босс сменил оружие на: {currentWeaponType}");
    }

    public void OnPhase2Start()
    {
        Debug.Log("⚡ Босс перешёл во вторую фазу!");
    }

    public void SetWeaponAndElement(WeaponType weapon, ElementType element)
    {
        currentWeaponType = weapon;
        currentElement = element;
        ApplyElementVisual();
        Debug.Log($"👑 Босс настроен: Оружие={weapon}, Стихия={element}");
    }
}