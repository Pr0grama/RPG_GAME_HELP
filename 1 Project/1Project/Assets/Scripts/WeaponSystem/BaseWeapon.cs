using UnityEngine;

public abstract class BaseWeapon : IWeapon
{
    public WeaponType WeaponType { get; protected set; }
    public ElementType Element { get; set; }
    public float Damage { get; protected set; }
    public float Cooldown { get; protected set; }
    public float Range { get; protected set; }

    protected GameObject effectPrefab;
    protected AudioClip attackSound;

    public virtual void Initialize(ElementType element)
    {
        Element = element;
        UpdateWeaponEffects();
    }

    public abstract void Attack(Transform attacker, Transform target);

    public void ChangeElement(ElementType newElement)
    {
        Element = newElement;
        UpdateWeaponEffects();
        Debug.Log($"⚔️ Оружие сменило стихию на: {Element}");
    }

    protected abstract void UpdateWeaponEffects();

    public GameObject GetEffectPrefab() => effectPrefab;
    public AudioClip GetAttackSound() => attackSound;
}