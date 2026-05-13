using UnityEngine;

public class RangedWeapon : BaseWeapon
{
    private GameObject projectilePrefab;

    public RangedWeapon()
    {
        WeaponType = WeaponType.Ranged;
        Damage = 25f;
        Cooldown = 2f;
        Range = 15f;
    }

    protected override void UpdateWeaponEffects()
    {
        switch (Element)
        {
            case ElementType.Fire:
                projectilePrefab = Resources.Load<GameObject>("Projectiles/FireBall");
                attackSound = Resources.Load<AudioClip>("Sounds/RangedFire");
                Damage = 30f;
                break;
            case ElementType.Ice:
                projectilePrefab = Resources.Load<GameObject>("Projectiles/IceBall");
                attackSound = Resources.Load<AudioClip>("Sounds/RangedIce");
                Damage = 25f;
                break;
            case ElementType.Earth:
                projectilePrefab = Resources.Load<GameObject>("Projectiles/EarthRock");
                attackSound = Resources.Load<AudioClip>("Sounds/RangedEarth");
                Damage = 35f;
                Cooldown = 3f;
                break;
            case ElementType.Ether:
                projectilePrefab = Resources.Load<GameObject>("Projectiles/EtherBall");
                attackSound = Resources.Load<AudioClip>("Sounds/RangedEther");
                Damage = 20f;
                Cooldown = 1.5f;
                break;
        }
    }

    public override void Attack(Transform attacker, Transform target)
    {
        if (target == null || projectilePrefab == null) return;

        // Проигрываем звук
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, attacker.position);
        }

        // Создаём снаряд
        GameObject projectile = Object.Instantiate(projectilePrefab, attacker.position + attacker.forward * 2, attacker.rotation);

        // Настраиваем снаряд
        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript == null)
            projScript = projectile.AddComponent<BossProjectile>();

        projScript.Initialize(Damage, Element, target, false, null);
    }
}