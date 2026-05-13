using UnityEngine;
using System.Collections;

public class BossWeaponController : MonoBehaviour
{
    [Header("Текущее оружие и стихия")]
    [SerializeField] private WeaponType currentWeaponType = WeaponType.Melee;
    [SerializeField] private ElementType currentElement = ElementType.Fire;

    [Header("Настройки смены стихии")]
    public float elementChangeInterval = 15f;
    public bool autoChangeElement = true;

    [Header("Настройки атак")]
    public float meleeDamage = 30f;
    public float rangedDamage = 25f;
    public float heavyMeleeDamage = 50f;
    public float heavyRangedDamage = 40f;

    [Header("Ссылки на префабы")]
    public GameObject meleeAttackEffect;   // Эффект для ближней атаки
    public GameObject rangedProjectilePrefab; // Снаряд для дальней атаки
    public GameObject heavyAttackEffect;   // Эффект для сильной атаки
    public Transform shootPoint;           // Точка вылета снаряда

    [Header("Звуки")]
    public AudioClip[] meleeSounds;
    public AudioClip[] rangedSounds;
    public AudioClip[] heavySounds;

    private float lastAttackTime = -2f;
    private float lastHeavyAttackTime = -4f;
    private AudioSource audioSource;

    public WeaponType CurrentWeaponType => currentWeaponType;
    public ElementType CurrentElement => currentElement;
    public float AttackCooldown => GetCurrentAttackCooldown();
    public float HeavyAttackCooldown => GetCurrentHeavyAttackCooldown();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Debug.Log($"👑 BossWeaponController инициализирован. Оружие: {currentWeaponType}, Стихия: {currentElement}");

        if (autoChangeElement)
        {
            StartCoroutine(AutoChangeElementRoutine());
        }
    }

    /// <summary>
    /// Проверка, можно ли выполнить обычную атаку
    /// </summary>
    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + GetCurrentAttackCooldown();
    }

    /// <summary>
    /// Проверка, можно ли выполнить сильную атаку
    /// </summary>
    public bool CanHeavyAttack()
    {
        return Time.time >= lastHeavyAttackTime + GetCurrentHeavyAttackCooldown();
    }

    /// <summary>
    /// Выполнение обычной атаки
    /// </summary>
    public void PerformAttack(Transform target)
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        if (currentWeaponType == WeaponType.Melee)
        {
            PerformMeleeAttack(target, false);
        }
        else
        {
            PerformRangedAttack(target, false);
        }
    }

    /// <summary>
    /// Выполнение сильной атаки
    /// </summary>
    public void PerformHeavyAttack(Transform target)
    {
        if (!CanHeavyAttack()) return;

        lastHeavyAttackTime = Time.time;

        if (currentWeaponType == WeaponType.Melee)
        {
            PerformMeleeAttack(target, true);
        }
        else
        {
            PerformRangedAttack(target, true);
        }
    }

    /// <summary>
    /// Ближняя атака
    /// </summary>
    private void PerformMeleeAttack(Transform target, bool isHeavy)
    {
        float damage = isHeavy ? heavyMeleeDamage : meleeDamage;

        // Наносим урон
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage, DamageType.Physical);
        }

        // Эффект атаки
        GameObject effect = isHeavy ? heavyAttackEffect : meleeAttackEffect;
        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, transform.position + transform.forward * 2, transform.rotation);
            Destroy(effectInstance, 0.5f);
        }

        // Звук
        AudioClip[] sounds = isHeavy ? heavySounds : meleeSounds;
        if (sounds != null && sounds.Length > 0)
        {
            AudioClip sound = sounds[Random.Range(0, sounds.Length)];
            if (sound != null)
                audioSource.PlayOneShot(sound);
        }

        // Применяем эффект стихии
        ApplyElementalEffect(target, isHeavy);

        Debug.Log($"👑 Босс атакует {currentElement} стихией! Урон: {damage}, {(isHeavy ? "СИЛЬНАЯ" : "обычная")} атака");
    }

    /// <summary>
    /// Дальняя атака
    /// </summary>
    private void PerformRangedAttack(Transform target, bool isHeavy)
    {
        if (rangedProjectilePrefab == null)
        {
            Debug.LogError("❌ rangedProjectilePrefab не назначен!");
            return;
        }

        if (shootPoint == null)
            shootPoint = transform;

        float damage = isHeavy ? heavyRangedDamage : rangedDamage;

        // Создаём снаряд
        GameObject projectile = Instantiate(rangedProjectilePrefab, shootPoint.position, shootPoint.rotation);

        // Настраиваем снаряд
        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript == null)
            projScript = projectile.AddComponent<BossProjectile>();

        projScript.Initialize(damage, currentElement, target, GetProjectileEffect());
        projScript.isHeavy = isHeavy;

        // Звук
        AudioClip[] sounds = isHeavy ? heavySounds : rangedSounds;
        if (sounds != null && sounds.Length > 0)
        {
            AudioClip sound = sounds[Random.Range(0, sounds.Length)];
            if (sound != null)
                audioSource.PlayOneShot(sound);
        }

        Debug.Log($"👑 Босс выпускает снаряд! Стихия: {currentElement}, Урон: {damage}");
    }

    /// <summary>
    /// Применение эффекта стихии к цели
    /// </summary>
    private void ApplyElementalEffect(Transform target, bool isHeavy)
    {
        // Здесь можно добавить дополнительные эффекты в зависимости от стихии
        switch (currentElement)
        {
            case ElementType.Fire:
                // Эффект поджигания
                Debug.Log($"🔥 Огненный урон! {target.name} горит!");
                break;
            case ElementType.Ice:
                // Эффект заморозки/замедления
                Debug.Log($"❄️ Ледяной урон! {target.name} заморожен!");
                break;
            case ElementType.Earth:
                // Эффект оглушения
                Debug.Log($"🪨 Земляной урон! {target.name} оглушён!");
                break;
            case ElementType.Ether:
                // Эффект отбрасывания
                Debug.Log($"✨ Эфирный урон! {target.name} отброшен!");
                break;
        }
    }

    /// <summary>
    /// Получение префаба эффекта для снаряда
    /// </summary>
    private GameObject GetProjectileEffect()
    {
        // Можно вернуть разные эффекты для разных стихий
        return meleeAttackEffect; // Заглушка
    }

    /// <summary>
    /// Смена типа оружия
    /// </summary>
    public void SwitchWeapon(WeaponType newType)
    {
        currentWeaponType = newType;
        Debug.Log($"⚔️ Босс сменил оружие на: {currentWeaponType}");
    }

    /// <summary>
    /// Смена стихии
    /// </summary>
    public void ChangeElement(ElementType newElement)
    {
        currentElement = newElement;
        Debug.Log($"✨ Босс сменил стихию на: {currentElement}");

        // Меняем цвет или визуал в зависимости от стихии
        UpdateBossAppearance();
    }

    /// <summary>
    /// При переходе во вторую фазу
    /// </summary>
    public void OnPhase2Start()
    {
        Debug.Log($"⚡ Босс активировал вторую фазу! Скорость атак увеличена!");
        // Здесь можно добавить визуальные эффекты перехода во вторую фазу
    }

    /// <summary>
    /// Обновление внешнего вида босса в зависимости от стихии
    /// </summary>
    private void UpdateBossAppearance()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color elementColor = GetElementColor();

        foreach (Renderer rend in renderers)
        {
            // Можно изменить цвет эмиссии или материала
            if (rend.material != null)
            {
                rend.material.SetColor("_EmissionColor", elementColor);
            }
        }
    }

    private Color GetElementColor()
    {
        switch (currentElement)
        {
            case ElementType.Fire: return Color.red;
            case ElementType.Ice: return Color.cyan;
            case ElementType.Earth: return Color.green;
            case ElementType.Ether: return Color.magenta;
            default: return Color.white;
        }
    }

    /// <summary>
    /// Кулдаун обычной атаки (зависит от стихии и фазы)
    /// </summary>
    private float GetCurrentAttackCooldown()
    {
        float baseCooldown = currentWeaponType == WeaponType.Melee ? 1.5f : 2f;

        // Эфир даёт ускорение
        if (currentElement == ElementType.Ether)
            baseCooldown *= 0.8f;

        return baseCooldown;
    }

    /// <summary>
    /// Кулдаун сильной атаки
    /// </summary>
    private float GetCurrentHeavyAttackCooldown()
    {
        float baseCooldown = currentWeaponType == WeaponType.Melee ? 4f : 5f;

        // Эфир даёт ускорение
        if (currentElement == ElementType.Ether)
            baseCooldown *= 0.7f;

        return baseCooldown;
    }

    /// <summary>
    /// Автоматическая смена стихии
    /// </summary>
    private IEnumerator AutoChangeElementRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(elementChangeInterval);

            // Выбираем случайную стихию, отличную от текущей
            ElementType newElement = GetRandomElement();
            while (newElement == currentElement)
            {
                newElement = GetRandomElement();
            }

            ChangeElement(newElement);
        }
    }

    private ElementType GetRandomElement()
    {
        System.Array values = System.Enum.GetValues(typeof(ElementType));
        return (ElementType)values.GetValue(Random.Range(0, values.Length));
    }

    /// <summary>
    /// Установка начального оружия и стихии
    /// </summary>
    public void SetWeaponAndElement(WeaponType weapon, ElementType element)
    {
        currentWeaponType = weapon;
        currentElement = element;
        Debug.Log($"👑 Босс настроен: Оружие={weapon}, Стихия={element}");
    }
}