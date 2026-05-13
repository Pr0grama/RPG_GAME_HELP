using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
    public MeleeWeapon()
    {
        WeaponType = WeaponType.Melee;
        Damage = 30f;
        Cooldown = 1.5f;
        Range = 3f;
    }

    protected override void UpdateWeaponEffects()
    {
        switch (Element)
        {
            case ElementType.Fire:
                effectPrefab = Resources.Load<GameObject>("Effects/FireSwing");
                attackSound = Resources.Load<AudioClip>("Sounds/MeleeFire");
                Damage = 35f;
                break;
            case ElementType.Ice:
                effectPrefab = Resources.Load<GameObject>("Effects/IceSwing");
                attackSound = Resources.Load<AudioClip>("Sounds/MeleeIce");
                Damage = 30f;
                break;
            case ElementType.Earth:
                effectPrefab = Resources.Load<GameObject>("Effects/EarthSwing");
                attackSound = Resources.Load<AudioClip>("Sounds/MeleeEarth");
                Damage = 40f;
                Cooldown = 2f; // Земля медленнее, но сильнее
                break;
            case ElementType.Ether:
                effectPrefab = Resources.Load<GameObject>("Effects/EtherSwing");
                attackSound = Resources.Load<AudioClip>("Sounds/MeleeEther");
                Damage = 25f;
                Cooldown = 1f; // Эфир быстрее
                break;
        }
    }

    public override void Attack(Transform attacker, Transform target)
    {
        if (target == null) return;

        // Создаём эффект
        if (effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(effectPrefab, attacker.position + attacker.forward * 2, attacker.rotation);
            Object.Destroy(effect, 0.5f);
        }

        // Проигрываем звук
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, attacker.position);
        }

        // Наносим урон
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(Damage, DamageType.Physical);
            Debug.Log($"💥 Босс атакует {Element} ближним оружием! Урон: {Damage}");
        }
    }
}